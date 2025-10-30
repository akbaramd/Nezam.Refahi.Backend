// Sidebar functionality
$(document).ready(function() {
    // Sidebar search functionality
    $('#sidebarSearch').on('input', function() {
        const searchTerm = $(this).val().toLowerCase();
        
        if (searchTerm === '') {
            // Show all menu items
            $('.menu-section').show();
            $('.submenu-item').show();
            return;
        }
        
        // Hide all menu sections first
        $('.menu-section').hide();
        
        // Show sections that match the search
        $('.menu-section').each(function() {
            const $section = $(this);
            const menuText = $section.find('.menu-text').text().toLowerCase();
            const submenuTexts = $section.find('.submenu-item span').map(function() {
                return $(this).text().toLowerCase();
            }).get();
            
            if (menuText.includes(searchTerm) || submenuTexts.some(text => text.includes(searchTerm))) {
                $section.show();
                
                // Highlight matching submenu items
                $section.find('.submenu-item').each(function() {
                    const $item = $(this);
                    const itemText = $item.find('span').text().toLowerCase();
                    
                    if (itemText.includes(searchTerm)) {
                        $item.show();
                    } else {
                        $item.hide();
                    }
                });
            }
        });
    });

    // Auto-expand active menu sections
    $('.menu-item.active').closest('.menu-section').find('.collapse').addClass('show');
    
    // Enhanced menu interactions
    $('.menu-item').on('click', function() {
        const $this = $(this);
        const $arrow = $this.find('.menu-arrow i');
        const $submenu = $this.next('.collapse');
        
        // Add click feedback
        $this.addClass('clicked');
        setTimeout(() => {
            $this.removeClass('clicked');
        }, 150);
        
        // Smooth arrow rotation
        if ($submenu.hasClass('show')) {
            $arrow.css('transform', 'rotate(0deg)');
        } else {
            $arrow.css('transform', 'rotate(90deg)');
        }
    });

    // Mobile sidebar functionality
    initializeMobileSidebar();
});

// Mobile sidebar functionality
function initializeMobileSidebar() {
    const hamburgerIcon = document.getElementById('topnav-hamburger-icon');
    const sidebar = document.querySelector('.app-menu');
    const overlay = document.querySelector('.vertical-overlay');
    
    if (!hamburgerIcon || !sidebar) return;

    // Check if we're on mobile
    function isMobile() {
        return window.innerWidth <= 1024;
    }

    // Show/hide hamburger icon based on screen size
    function toggleHamburgerVisibility() {
        if (isMobile()) {
            hamburgerIcon.style.display = 'block';
        } else {
            hamburgerIcon.style.display = 'none';
            // Ensure sidebar is visible on desktop
            sidebar.classList.remove('mobile-open');
            document.body.classList.remove('sidebar-open');
            if (overlay) overlay.style.display = 'none';
        }
    }

    // Open sidebar on mobile
    function openMobileSidebar() {
        if (!isMobile()) return;
        
        sidebar.classList.add('mobile-open');
        document.body.classList.add('sidebar-open');
        if (overlay) {
            overlay.style.display = 'block';
            overlay.style.opacity = '1';
        }
        
        // Prevent body scroll
        document.body.style.overflow = 'hidden';
    }

    // Close sidebar on mobile
    function closeMobileSidebar() {
        if (!isMobile()) return;
        
        sidebar.classList.remove('mobile-open');
        document.body.classList.remove('sidebar-open');
        if (overlay) {
            overlay.style.opacity = '0';
            setTimeout(() => {
                overlay.style.display = 'none';
            }, 300);
        }
        
        // Restore body scroll
        document.body.style.overflow = '';
    }

    // Toggle sidebar
    function toggleMobileSidebar() {
        if (sidebar.classList.contains('mobile-open')) {
            closeMobileSidebar();
        } else {
            openMobileSidebar();
        }
    }

    // Event listeners
    hamburgerIcon.addEventListener('click', function(e) {
        e.preventDefault();
        e.stopPropagation();
        toggleMobileSidebar();
    });

    // Close sidebar when clicking overlay
    if (overlay) {
        overlay.addEventListener('click', closeMobileSidebar);
    }

    // Close sidebar when clicking outside
    document.addEventListener('click', function(e) {
        if (isMobile() && sidebar.classList.contains('mobile-open')) {
            if (!sidebar.contains(e.target) && !hamburgerIcon.contains(e.target)) {
                closeMobileSidebar();
            }
        }
    });

    // Handle window resize
    window.addEventListener('resize', function() {
        toggleHamburgerVisibility();
        
        // If switching to desktop, ensure sidebar is properly positioned
        if (!isMobile()) {
            closeMobileSidebar();
        }
    });

    // Handle escape key
    document.addEventListener('keydown', function(e) {
        if (e.key === 'Escape' && isMobile() && sidebar.classList.contains('mobile-open')) {
            closeMobileSidebar();
        }
    });

    // Initialize hamburger visibility
    toggleHamburgerVisibility();
    
    // Auto-close sidebar when clicking on menu items (mobile only)
    // But don't close when clicking on parent items with submenus
    $('.submenu-item').on('click', function() {
        if (isMobile()) {
            setTimeout(() => {
                closeMobileSidebar();
            }, 300);
        }
    });
    
    // For parent menu items, only close if they don't have submenus
    $('.menu-item').on('click', function() {
        const $this = $(this);
        const hasSubmenu = $this.next('.collapse').length > 0;
        
        // Only close sidebar if this menu item doesn't have a submenu
        if (isMobile() && !hasSubmenu) {
            setTimeout(() => {
                closeMobileSidebar();
            }, 300);
        }
    });
    
    // Override default hamburger menu behavior
    overrideDefaultHamburgerBehavior();
}

// Override the default hamburger menu behavior from app.js
function overrideDefaultHamburgerBehavior() {
    const hamburgerIcon = document.getElementById('topnav-hamburger-icon');
    if (!hamburgerIcon) return;
    
    // Remove existing event listeners
    const newHamburgerIcon = hamburgerIcon.cloneNode(true);
    hamburgerIcon.parentNode.replaceChild(newHamburgerIcon, hamburgerIcon);
    
    // Add our custom event listener
    newHamburgerIcon.addEventListener('click', function(e) {
        e.preventDefault();
        e.stopPropagation();
        
        // Only handle on mobile
        if (window.innerWidth <= 1024) {
            const sidebar = document.querySelector('.app-menu');
            if (sidebar) {
                if (sidebar.classList.contains('mobile-open')) {
                    sidebar.classList.remove('mobile-open');
                    document.body.classList.remove('sidebar-open');
                    const overlay = document.querySelector('.vertical-overlay');
                    if (overlay) {
                        overlay.style.opacity = '0';
                        setTimeout(() => {
                            overlay.style.display = 'none';
                        }, 300);
                    }
                    document.body.style.overflow = '';
                } else {
                    sidebar.classList.add('mobile-open');
                    document.body.classList.add('sidebar-open');
                    const overlay = document.querySelector('.vertical-overlay');
                    if (overlay) {
                        overlay.style.display = 'block';
                        overlay.style.opacity = '1';
                    }
                    document.body.style.overflow = 'hidden';
                }
            }
        }
    });
}
