/*  wwwroot/js/select2.init.js
    Load *once* after jquery.min.js and select2.min.js
------------------------------------------------------*/
$(function () {

    /** build Select2 config object from data-* attributes */
    const buildCfg = $el => {
        const cfg = {
            placeholder: $el.data('s2-placeholder'),
            minimumInputLength: $el.data('s2-minlen') || 0,
            allowClear: !!$el.data('s2-allowclear'),
            multiple:   !!$el.data('s2-multiple'),
            dir: "rtl",
            language: "fa",
            tags:       !!$el.data('s2-tags'),
            ajax: {
                delay: 250,
                url: $el.data('s2-url'),
                data: params => ({
                    q: params.term || '',
                    page: params.page || 1,
                    pageSize: $el.data('s2-pagesize') || 20,
                    ...($el.data('s2-extra') || {})
                }),
                processResults: (d,p) => ({
                    results: d.results,
                    pagination: { more: d.pagination?.more }
                })
            }
        };
        console.log(cfg)
        // dependency glue
        const parentSel = $el.data('s2-dependson');
        if (parentSel) {
            const qp = $el.data('s2-dependsparam') || 'parentId';

            cfg.ajax.data = params => ({
                q: params.term || '',
                page: params.page || 1,
                pageSize: $el.data('s2-pagesize') || 20,
                [qp]: $(parentSel).val() || '',
                ...($el.data('s2-extra') || {})
            });

            const toggle = () => $el.prop('disabled', !$(parentSel).val());
            $(parentSel).on('change', () => { $el.val(null).trigger('change'); toggle(); });
            toggle();
        }
        return cfg;
    };

    /** initialise one element (idempotent) */
    const initOne = el => {
        const $el = $(el);
        console.log($el);
        if ($el.data('s2-init')) return;
        $el.select2(buildCfg($el));
        $el.data('s2-init', 1);
    };

    /** initial boot + dynamic observer */
    const bootAll = root => $(root).find('[data-s2-select="true"]').each((_,el)=>initOne(el));

    // first pass
    bootAll(document);

    // watch DOM changes (for HTMX/partial updates)
    new MutationObserver(muts =>
        muts.flatMap(m => [...m.addedNodes])
            .filter(n => n.nodeType === 1)
            .forEach(n => { if (n.hasAttribute?.('data-s2-select')) initOne(n); bootAll(n); })
    ).observe(document.body, { childList:true, subtree:true });
});
