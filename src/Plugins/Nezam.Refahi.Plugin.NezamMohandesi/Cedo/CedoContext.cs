using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo;

public partial class CedoContext : DbContext
{
    public CedoContext()
    {
    }

    public CedoContext(DbContextOptions<CedoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AcceptedPlanControl> AcceptedPlanControls { get; set; }

    public virtual DbSet<ActivityLicense> ActivityLicenses { get; set; }

    public virtual DbSet<ActivityTransferRequest> ActivityTransferRequests { get; set; }

    public virtual DbSet<ActivityTransferRequestStatus> ActivityTransferRequestStatuses { get; set; }

    public virtual DbSet<AdditionalSupervisionField> AdditionalSupervisionFields { get; set; }

    public virtual DbSet<ArchitectureExecutiveOperationsForm> ArchitectureExecutiveOperationsForms { get; set; }

    public virtual DbSet<ArchitecturePlanControlForm> ArchitecturePlanControlForms { get; set; }

    public virtual DbSet<ArchitectureRedLinesForm> ArchitectureRedLinesForms { get; set; }

    public virtual DbSet<ArchitectureSafetyForm> ArchitectureSafetyForms { get; set; }

    public virtual DbSet<ArchitectureSpecForm> ArchitectureSpecForms { get; set; }

    public virtual DbSet<AsBuiltRequest> AsBuiltRequests { get; set; }

    public virtual DbSet<AsBuiltRequestStatus> AsBuiltRequestStatuses { get; set; }

    public virtual DbSet<Attendee> Attendees { get; set; }

    public virtual DbSet<AttendeeOption> AttendeeOptions { get; set; }

    public virtual DbSet<AttendeeSession> AttendeeSessions { get; set; }

    public virtual DbSet<AttendeeSessionStatus> AttendeeSessionStatuses { get; set; }

    public virtual DbSet<AttendeeStatus> AttendeeStatuses { get; set; }

    public virtual DbSet<AttributePropertyValue> AttributePropertyValues { get; set; }

    public virtual DbSet<BankAcountType> BankAcountTypes { get; set; }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<BillDetail> BillDetails { get; set; }

    public virtual DbSet<BillDetailItem> BillDetailItems { get; set; }

    public virtual DbSet<BillType> BillTypes { get; set; }

    public virtual DbSet<BoundaryEvent> BoundaryEvents { get; set; }

    public virtual DbSet<BuildingCertificate> BuildingCertificates { get; set; }

    public virtual DbSet<BuildingCertificateTimeout> BuildingCertificateTimeouts { get; set; }

    public virtual DbSet<BuildingGroup> BuildingGroups { get; set; }

    public virtual DbSet<BuildingGroupSetting> BuildingGroupSettings { get; set; }

    public virtual DbSet<BuildingGroupSettingCity> BuildingGroupSettingCities { get; set; }

    public virtual DbSet<BuildingGroupSettingRatio> BuildingGroupSettingRatios { get; set; }

    public virtual DbSet<BuildingSubGroup> BuildingSubGroups { get; set; }

    public virtual DbSet<BuildingUnit> BuildingUnits { get; set; }

    public virtual DbSet<BuildingWorker> BuildingWorkers { get; set; }

    public virtual DbSet<CancelDossierBill> CancelDossierBills { get; set; }

    public virtual DbSet<CancelDossierBillDetail> CancelDossierBillDetails { get; set; }

    public virtual DbSet<CancelDossierRequest> CancelDossierRequests { get; set; }

    public virtual DbSet<CancelDossierStatus> CancelDossierStatuses { get; set; }

    public virtual DbSet<CancelEvent> CancelEvents { get; set; }

    public virtual DbSet<CapacityCalculation> CapacityCalculations { get; set; }

    public virtual DbSet<CategoryManpowerSkillCard> CategoryManpowerSkillCards { get; set; }

    public virtual DbSet<CedoAgency> CedoAgencies { get; set; }

    public virtual DbSet<CedoAgencyCity> CedoAgencyCities { get; set; }

    public virtual DbSet<ChangeDesignerRequest> ChangeDesignerRequests { get; set; }

    public virtual DbSet<ChangeDesignerRequestStatus> ChangeDesignerRequestStatuses { get; set; }

    public virtual DbSet<ChangeExecuterRequest> ChangeExecuterRequests { get; set; }

    public virtual DbSet<ChangeExecuterRequestStatus> ChangeExecuterRequestStatuses { get; set; }

    public virtual DbSet<ChangeOwnerRequest> ChangeOwnerRequests { get; set; }

    public virtual DbSet<ChangeOwnerRequestDetail> ChangeOwnerRequestDetails { get; set; }

    public virtual DbSet<ChangeOwnerStatus> ChangeOwnerStatuses { get; set; }

    public virtual DbSet<ChangePlanFeasibility> ChangePlanFeasibilities { get; set; }

    public virtual DbSet<ChangePlanFeasibilityJudgement> ChangePlanFeasibilityJudgements { get; set; }

    public virtual DbSet<ChangePlanRequest> ChangePlanRequests { get; set; }

    public virtual DbSet<ChangePlanRequestStatus> ChangePlanRequestStatuses { get; set; }

    public virtual DbSet<ChangeSiteManagerRequest> ChangeSiteManagerRequests { get; set; }

    public virtual DbSet<ChangeSiteManagerStatus> ChangeSiteManagerStatuses { get; set; }

    public virtual DbSet<ChangeSupervisorJudgment> ChangeSupervisorJudgments { get; set; }

    public virtual DbSet<ChangeSupervisorRequest> ChangeSupervisorRequests { get; set; }

    public virtual DbSet<ChangeSupervisorRequestStatus> ChangeSupervisorRequestStatuses { get; set; }

    public virtual DbSet<CheckUser> CheckUsers { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Complainee> Complainees { get; set; }

    public virtual DbSet<Complaint> Complaints { get; set; }

    public virtual DbSet<ComplaintCommunicated> ComplaintCommunicateds { get; set; }

    public virtual DbSet<ComplaintMember> ComplaintMembers { get; set; }

    public virtual DbSet<ComplaintMemberType> ComplaintMemberTypes { get; set; }

    public virtual DbSet<ComplaintResultMember> ComplaintResultMembers { get; set; }

    public virtual DbSet<ComplaintResultStatus> ComplaintResultStatuses { get; set; }

    public virtual DbSet<ComplaintReview> ComplaintReviews { get; set; }

    public virtual DbSet<ComplaintReviewType> ComplaintReviewTypes { get; set; }

    public virtual DbSet<ComplaintStatus> ComplaintStatuses { get; set; }

    public virtual DbSet<ConstructionLicense> ConstructionLicenses { get; set; }

    public virtual DbSet<ConstructionLicenseArea> ConstructionLicenseAreas { get; set; }

    public virtual DbSet<ConstructionLicenseComment> ConstructionLicenseComments { get; set; }

    public virtual DbSet<ConstructionLicenseDescription> ConstructionLicenseDescriptions { get; set; }

    public virtual DbSet<ConstructionLicenseDocument> ConstructionLicenseDocuments { get; set; }

    public virtual DbSet<ConstructionLicenseGeoLab> ConstructionLicenseGeoLabs { get; set; }

    public virtual DbSet<ConstructionLicenseOption> ConstructionLicenseOptions { get; set; }

    public virtual DbSet<ConstructionLicenseStatus> ConstructionLicenseStatuses { get; set; }

    public virtual DbSet<ConstructionLicenseStep> ConstructionLicenseSteps { get; set; }

    public virtual DbSet<ConstructionLicenseType> ConstructionLicenseTypes { get; set; }

    public virtual DbSet<ContactInfoType> ContactInfoTypes { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<ContractExpandation> ContractExpandations { get; set; }

    public virtual DbSet<ContractExpandationStatus> ContractExpandationStatuses { get; set; }

    public virtual DbSet<ContractPaymentDetail> ContractPaymentDetails { get; set; }

    public virtual DbSet<ContractPaymentFor> ContractPaymentFors { get; set; }

    public virtual DbSet<ContractPaymentType> ContractPaymentTypes { get; set; }

    public virtual DbSet<ContractStatus> ContractStatuses { get; set; }

    public virtual DbSet<ContractType> ContractTypes { get; set; }

    public virtual DbSet<Controller> Controllers { get; set; }

    public virtual DbSet<ControllerAction> ControllerActions { get; set; }

    public virtual DbSet<ControllerActionCode> ControllerActionCodes { get; set; }

    public virtual DbSet<ControllerActionType> ControllerActionTypes { get; set; }

    public virtual DbSet<ControllerType> ControllerTypes { get; set; }

    public virtual DbSet<CostType> CostTypes { get; set; }

    public virtual DbSet<CostTypeDetail> CostTypeDetails { get; set; }

    public virtual DbSet<CostTypeDetailBill> CostTypeDetailBills { get; set; }

    public virtual DbSet<CostTypeDetailSetting> CostTypeDetailSettings { get; set; }

    public virtual DbSet<CostTypeOrder> CostTypeOrders { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseExecutor> CourseExecutors { get; set; }

    public virtual DbSet<CourseHolding> CourseHoldings { get; set; }

    public virtual DbSet<CourseHoldingMember> CourseHoldingMembers { get; set; }

    public virtual DbSet<CourseHoldingSignatoriese> CourseHoldingSignatorieses { get; set; }

    public virtual DbSet<CourseHoldingStep> CourseHoldingSteps { get; set; }

    public virtual DbSet<CourseLesson> CourseLessons { get; set; }

    public virtual DbSet<CourseQualifiication> CourseQualifiications { get; set; }

    public virtual DbSet<CourseType> CourseTypes { get; set; }

    public virtual DbSet<DashboardGadget> DashboardGadgets { get; set; }

    public virtual DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    public virtual DbSet<DataSourceDiagram> DataSourceDiagrams { get; set; }

    public virtual DbSet<DataSourceEntity> DataSourceEntities { get; set; }

    public virtual DbSet<DataSourceType> DataSourceTypes { get; set; }

    public virtual DbSet<DecisionTable> DecisionTables { get; set; }

    public virtual DbSet<DecisionTableTask> DecisionTableTasks { get; set; }

    public virtual DbSet<DeletedAttendee> DeletedAttendees { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<DepartmentUser> DepartmentUsers { get; set; }

    public virtual DbSet<DeploymentSide> DeploymentSides { get; set; }

    public virtual DbSet<DescriptionType> DescriptionTypes { get; set; }

    public virtual DbSet<DiagramEntity> DiagramEntities { get; set; }

    public virtual DbSet<DocumentFinder> DocumentFinders { get; set; }

    public virtual DbSet<DocumentIndex> DocumentIndices { get; set; }

    public virtual DbSet<DocumentType> DocumentTypes { get; set; }

    public virtual DbSet<DossierBuildingGroupSetting> DossierBuildingGroupSettings { get; set; }

    public virtual DbSet<DossierCostSetting> DossierCostSettings { get; set; }

    public virtual DbSet<DossierCostSettingDetail> DossierCostSettingDetails { get; set; }

    public virtual DbSet<DossierType> DossierTypes { get; set; }

    public virtual DbSet<DrillingType> DrillingTypes { get; set; }

    public virtual DbSet<DuplicatedMember> DuplicatedMembers { get; set; }

    public virtual DbSet<ElectricalExecutiveOperationsForm> ElectricalExecutiveOperationsForms { get; set; }

    public virtual DbSet<ElectricalPlanControlForm> ElectricalPlanControlForms { get; set; }

    public virtual DbSet<ElectricalRedLinesForm> ElectricalRedLinesForms { get; set; }

    public virtual DbSet<ElectricalSpecForm> ElectricalSpecForms { get; set; }

    public virtual DbSet<ElectricitySafetyForm> ElectricitySafetyForms { get; set; }

    public virtual DbSet<Elevator> Elevators { get; set; }

    public virtual DbSet<EmailAccount> EmailAccounts { get; set; }

    public virtual DbSet<EmploymentHistory> EmploymentHistories { get; set; }

    public virtual DbSet<EngServiceCostScaleSetting> EngServiceCostScaleSettings { get; set; }

    public virtual DbSet<EngServiceTariff> EngServiceTariffs { get; set; }

    public virtual DbSet<EngServiceTariffDetail> EngServiceTariffDetails { get; set; }

    public virtual DbSet<EngServiceTariffDetailItem> EngServiceTariffDetailItems { get; set; }

    public virtual DbSet<EngineeringGrade> EngineeringGrades { get; set; }

    public virtual DbSet<EntityProperty> EntityProperties { get; set; }

    public virtual DbSet<EnumEntityItem> EnumEntityItems { get; set; }

    public virtual DbSet<ErrorCatchEvent> ErrorCatchEvents { get; set; }

    public virtual DbSet<Estate> Estates { get; set; }

    public virtual DbSet<EstateCoordinate> EstateCoordinates { get; set; }

    public virtual DbSet<Examination> Examinations { get; set; }

    public virtual DbSet<ExaminationPlace> ExaminationPlaces { get; set; }

    public virtual DbSet<ExaminationStatus> ExaminationStatuses { get; set; }

    public virtual DbSet<ExaminationType> ExaminationTypes { get; set; }

    public virtual DbSet<ExcavationDangerStatus> ExcavationDangerStatuses { get; set; }

    public virtual DbSet<ExecuterCompetenceSetting> ExecuterCompetenceSettings { get; set; }

    public virtual DbSet<ExecuterContract> ExecuterContracts { get; set; }

    public virtual DbSet<ExecuterContractSuspensionRequest> ExecuterContractSuspensionRequests { get; set; }

    public virtual DbSet<ExecuterContractSuspensionRequestStatus> ExecuterContractSuspensionRequestStatuses { get; set; }

    public virtual DbSet<ExecuterContractTermination> ExecuterContractTerminations { get; set; }

    public virtual DbSet<ExecuterContractTerminationJudgment> ExecuterContractTerminationJudgments { get; set; }

    public virtual DbSet<ExecuterContractTerminationStatus> ExecuterContractTerminationStatuses { get; set; }

    public virtual DbSet<ExpertCostsSetting> ExpertCostsSettings { get; set; }

    public virtual DbSet<ExternalReport> ExternalReports { get; set; }

    public virtual DbSet<ExternalReportStatus> ExternalReportStatuses { get; set; }

    public virtual DbSet<FacilityDeviceType> FacilityDeviceTypes { get; set; }

    public virtual DbSet<FacilityDevicesSpecification> FacilityDevicesSpecifications { get; set; }

    public virtual DbSet<FacilityRequest> FacilityRequests { get; set; }

    public virtual DbSet<FacilityRequestFor> FacilityRequestFors { get; set; }

    public virtual DbSet<FacilityRequestStatus> FacilityRequestStatuses { get; set; }

    public virtual DbSet<Firewall> Firewalls { get; set; }

    public virtual DbSet<FirewallRule> FirewallRules { get; set; }

    public virtual DbSet<FiscalBalance> FiscalBalances { get; set; }

    public virtual DbSet<Floor> Floors { get; set; }

    public virtual DbSet<Floor1> Floors1 { get; set; }

    public virtual DbSet<FloorRegisterStep> FloorRegisterSteps { get; set; }

    public virtual DbSet<FloorType> FloorTypes { get; set; }

    public virtual DbSet<Flow> Flows { get; set; }

    public virtual DbSet<FlowActor> FlowActors { get; set; }

    public virtual DbSet<FlowConnectionType> FlowConnectionTypes { get; set; }

    public virtual DbSet<FlowConnector> FlowConnectors { get; set; }

    public virtual DbSet<FlowEvent> FlowEvents { get; set; }

    public virtual DbSet<FlowEventMode> FlowEventModes { get; set; }

    public virtual DbSet<FlowEventTrigger> FlowEventTriggers { get; set; }

    public virtual DbSet<FlowEventType> FlowEventTypes { get; set; }

    public virtual DbSet<FlowGatewaie> FlowGatewaies { get; set; }

    public virtual DbSet<FlowInstance> FlowInstances { get; set; }

    public virtual DbSet<FlowInstanceLog> FlowInstanceLogs { get; set; }

    public virtual DbSet<FlowInstancePauseLog> FlowInstancePauseLogs { get; set; }

    public virtual DbSet<FlowInstanceStatus> FlowInstanceStatuses { get; set; }

    public virtual DbSet<FlowInstanceVariable> FlowInstanceVariables { get; set; }

    public virtual DbSet<FlowNode> FlowNodes { get; set; }

    public virtual DbSet<FlowNodeLifeCycleEvent> FlowNodeLifeCycleEvents { get; set; }

    public virtual DbSet<FlowNodeLifeCycleEventType> FlowNodeLifeCycleEventTypes { get; set; }

    public virtual DbSet<FlowNodeType> FlowNodeTypes { get; set; }

    public virtual DbSet<FlowStarterTask> FlowStarterTasks { get; set; }

    public virtual DbSet<FlowStep> FlowSteps { get; set; }

    public virtual DbSet<FlowStepStatus> FlowStepStatuses { get; set; }

    public virtual DbSet<FlowVersion> FlowVersions { get; set; }

    public virtual DbSet<FormEditTask> FormEditTasks { get; set; }

    public virtual DbSet<FormEntryTask> FormEntryTasks { get; set; }

    public virtual DbSet<GasBranchOffice> GasBranchOffices { get; set; }

    public virtual DbSet<GasBranchUser> GasBranchUsers { get; set; }

    public virtual DbSet<GasCounterType> GasCounterTypes { get; set; }

    public virtual DbSet<GasOncallStatus> GasOncallStatuses { get; set; }

    public virtual DbSet<GasRequest> GasRequests { get; set; }

    public virtual DbSet<GasRequestAuditForm> GasRequestAuditForms { get; set; }

    public virtual DbSet<GasRequestInvolvedMember> GasRequestInvolvedMembers { get; set; }

    public virtual DbSet<GasRequestInvolvedMemberChangeLog> GasRequestInvolvedMemberChangeLogs { get; set; }

    public virtual DbSet<GasRequestOncall> GasRequestOncalls { get; set; }

    public virtual DbSet<GasRequestOncallDescription> GasRequestOncallDescriptions { get; set; }

    public virtual DbSet<GasRequestOncallStatus> GasRequestOncallStatuses { get; set; }

    public virtual DbSet<GasRequestScopeType> GasRequestScopeTypes { get; set; }

    public virtual DbSet<GasRequestSplit> GasRequestSplits { get; set; }

    public virtual DbSet<GasRequestSplitDocument> GasRequestSplitDocuments { get; set; }

    public virtual DbSet<GasRequestSplitProblem> GasRequestSplitProblems { get; set; }

    public virtual DbSet<GasRequestStatus> GasRequestStatuses { get; set; }

    public virtual DbSet<GasRequestType> GasRequestTypes { get; set; }

    public virtual DbSet<GasRotatingForm> GasRotatingForms { get; set; }

    public virtual DbSet<GasSubscriptionType> GasSubscriptionTypes { get; set; }

    public virtual DbSet<GasSupervisionResult> GasSupervisionResults { get; set; }

    public virtual DbSet<GasSupervisionResultState> GasSupervisionResultStates { get; set; }

    public virtual DbSet<GasUsageType> GasUsageTypes { get; set; }

    public virtual DbSet<GradeCalculationSetting> GradeCalculationSettings { get; set; }

    public virtual DbSet<HealthStatus> HealthStatuses { get; set; }

    public virtual DbSet<HoldingLesson> HoldingLessons { get; set; }

    public virtual DbSet<HoldingLessonOption> HoldingLessonOptions { get; set; }

    public virtual DbSet<HoldingLessonSession> HoldingLessonSessions { get; set; }

    public virtual DbSet<HoldingLessonStatus> HoldingLessonStatuses { get; set; }

    public virtual DbSet<IncludedResource> IncludedResources { get; set; }

    public virtual DbSet<IncludedResourceType> IncludedResourceTypes { get; set; }

    public virtual DbSet<InstanceUserGroup> InstanceUserGroups { get; set; }

    public virtual DbSet<InstanceUserGroupMember> InstanceUserGroupMembers { get; set; }

    public virtual DbSet<IntroductionRequest> IntroductionRequests { get; set; }

    public virtual DbSet<IntroductionRequestStatus> IntroductionRequestStatuses { get; set; }

    public virtual DbSet<IntroductionTarget> IntroductionTargets { get; set; }

    public virtual DbSet<InvolvedMember> InvolvedMembers { get; set; }

    public virtual DbSet<InvolvedMemberStatus> InvolvedMemberStatuses { get; set; }

    public virtual DbSet<InvolvedMemberType> InvolvedMemberTypes { get; set; }

    public virtual DbSet<LegalMember> LegalMembers { get; set; }

    public virtual DbSet<LegalMemberPersonnel> LegalMemberPersonnels { get; set; }

    public virtual DbSet<LegalMemberType> LegalMemberTypes { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<LicenseCapacity> LicenseCapacities { get; set; }

    public virtual DbSet<LicenseCapacityRemaining> LicenseCapacityRemainings { get; set; }

    public virtual DbSet<LicenseInquery> LicenseInqueries { get; set; }

    public virtual DbSet<LicenseInqueryStatus> LicenseInqueryStatuses { get; set; }

    public virtual DbSet<LocalizablePhrase> LocalizablePhrases { get; set; }

    public virtual DbSet<LogType> LogTypes { get; set; }

    public virtual DbSet<ManpowerSkillCard> ManpowerSkillCards { get; set; }

    public virtual DbSet<ManpowerSkillCardValue> ManpowerSkillCardValues { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<MaterialCategory> MaterialCategories { get; set; }

    public virtual DbSet<MaterialValue> MaterialValues { get; set; }

    public virtual DbSet<MechanicSafetyForm> MechanicSafetyForms { get; set; }

    public virtual DbSet<MechanicalExecutiveOperationsForm> MechanicalExecutiveOperationsForms { get; set; }

    public virtual DbSet<MechanicalPlanControlForm> MechanicalPlanControlForms { get; set; }

    public virtual DbSet<MechanicalRedLinesForm> MechanicalRedLinesForms { get; set; }

    public virtual DbSet<MechanicalSpecForm> MechanicalSpecForms { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MemberBankAcount> MemberBankAcounts { get; set; }

    public virtual DbSet<MemberCapacity> MemberCapacities { get; set; }

    public virtual DbSet<MemberCapacityDetail> MemberCapacityDetails { get; set; }

    public virtual DbSet<MemberContact> MemberContacts { get; set; }

    public virtual DbSet<MemberService> MemberServices { get; set; }

    public virtual DbSet<MemberShipType> MemberShipTypes { get; set; }

    public virtual DbSet<MemberType> MemberTypes { get; set; }

    public virtual DbSet<MemberWorkField> MemberWorkFields { get; set; }

    public virtual DbSet<MenuItem> MenuItems { get; set; }

    public virtual DbSet<MenuItemExternalLinkSetting> MenuItemExternalLinkSettings { get; set; }

    public virtual DbSet<MenuItemFlowLinkSetting> MenuItemFlowLinkSettings { get; set; }

    public virtual DbSet<MenuItemViewLinkSetting> MenuItemViewLinkSettings { get; set; }

    public virtual DbSet<MessageCatchEvent> MessageCatchEvents { get; set; }

    public virtual DbSet<MessageThrowEvent> MessageThrowEvents { get; set; }

    public virtual DbSet<Municipality> Municipalities { get; set; }

    public virtual DbSet<NodeAnnotationLayout> NodeAnnotationLayouts { get; set; }

    public virtual DbSet<NodeShapeStyle> NodeShapeStyles { get; set; }

    public virtual DbSet<NodeTask> NodeTasks { get; set; }

    public virtual DbSet<OnlinePaymentSetting> OnlinePaymentSettings { get; set; }

    public virtual DbSet<Operator> Operators { get; set; }

    public virtual DbSet<Operator1> Operators1 { get; set; }

    public virtual DbSet<OperatorType> OperatorTypes { get; set; }

    public virtual DbSet<OrgPersonnel> OrgPersonnels { get; set; }

    public virtual DbSet<OrgPost> OrgPosts { get; set; }

    public virtual DbSet<OrgUnit> OrgUnits { get; set; }

    public virtual DbSet<OrgUnitPersonnel> OrgUnitPersonnels { get; set; }

    public virtual DbSet<OrgUnitType> OrgUnitTypes { get; set; }

    public virtual DbSet<OrganizationGroup> OrganizationGroups { get; set; }

    public virtual DbSet<OrganizationPost> OrganizationPosts { get; set; }

    public virtual DbSet<OrganizationUnit> OrganizationUnits { get; set; }

    public virtual DbSet<Owner> Owners { get; set; }

    public virtual DbSet<OwnerType> OwnerTypes { get; set; }

    public virtual DbSet<OwnershipDocumentType> OwnershipDocumentTypes { get; set; }

    public virtual DbSet<PageMetaTag> PageMetaTags { get; set; }

    public virtual DbSet<ParaRole> ParaRoles { get; set; }

    public virtual DbSet<ParaRoleClaim> ParaRoleClaims { get; set; }

    public virtual DbSet<ParaUser> ParaUsers { get; set; }

    public virtual DbSet<ParaUserClaim> ParaUserClaims { get; set; }

    public virtual DbSet<ParaUserLogin> ParaUserLogins { get; set; }

    public virtual DbSet<ParaUserToken> ParaUserTokens { get; set; }

    public virtual DbSet<PasswayType> PasswayTypes { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentComeBackRequest> PaymentComeBackRequests { get; set; }

    public virtual DbSet<PaymentReceipt> PaymentReceipts { get; set; }

    public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }

    public virtual DbSet<PaymentType> PaymentTypes { get; set; }

    public virtual DbSet<Personel> Personels { get; set; }

    public virtual DbSet<PersonelDossier> PersonelDossiers { get; set; }

    public virtual DbSet<PersonelGroup> PersonelGroups { get; set; }

    public virtual DbSet<PersonelPost> PersonelPosts { get; set; }

    public virtual DbSet<PersonnelPost> PersonnelPosts { get; set; }

    public virtual DbSet<PlanControl> PlanControls { get; set; }

    public virtual DbSet<PlanControlSetting> PlanControlSettings { get; set; }

    public virtual DbSet<PlanControlStatus> PlanControlStatuses { get; set; }

    public virtual DbSet<PreviousStep> PreviousSteps { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectCodeDirectory> ProjectCodeDirectories { get; set; }

    public virtual DbSet<ProjectCodeFile> ProjectCodeFiles { get; set; }

    public virtual DbSet<ProjectDataSource> ProjectDataSources { get; set; }

    public virtual DbSet<ProjectHelp> ProjectHelps { get; set; }

    public virtual DbSet<ProjectLoginSetting> ProjectLoginSettings { get; set; }

    public virtual DbSet<ProjectMenu> ProjectMenus { get; set; }

    public virtual DbSet<ProjectRecentDocument> ProjectRecentDocuments { get; set; }

    public virtual DbSet<ProjectTranslation> ProjectTranslations { get; set; }

    public virtual DbSet<ProjectView> ProjectViews { get; set; }

    public virtual DbSet<ProjectWebPushSetting> ProjectWebPushSettings { get; set; }

    public virtual DbSet<ProjectWorkItemSetting> ProjectWorkItemSettings { get; set; }

    public virtual DbSet<PropertyAttribute> PropertyAttributes { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<PwaPreCacheAction> PwaPreCacheActions { get; set; }

    public virtual DbSet<PwaSetting> PwaSettings { get; set; }

    public virtual DbSet<PwaShortcutAction> PwaShortcutActions { get; set; }

    public virtual DbSet<RealMember> RealMembers { get; set; }

    public virtual DbSet<RejectedPlanControl> RejectedPlanControls { get; set; }

    public virtual DbSet<RejectedPlanControlsWithArea> RejectedPlanControlsWithAreas { get; set; }

    public virtual DbSet<RequestStatus> RequestStatuses { get; set; }

    public virtual DbSet<RequestType> RequestTypes { get; set; }

    public virtual DbSet<RoleMenu> RoleMenus { get; set; }

    public virtual DbSet<RuleDestination> RuleDestinations { get; set; }

    public virtual DbSet<RuleDeviceSource> RuleDeviceSources { get; set; }

    public virtual DbSet<RuleIdentitySource> RuleIdentitySources { get; set; }

    public virtual DbSet<RuleRoleSource> RuleRoleSources { get; set; }

    public virtual DbSet<RuleSubnetSource> RuleSubnetSources { get; set; }

    public virtual DbSet<RuleTimeSource> RuleTimeSources { get; set; }

    public virtual DbSet<RuleUserSource> RuleUserSources { get; set; }

    public virtual DbSet<SafetyReport> SafetyReports { get; set; }

    public virtual DbSet<SafetyReportResult> SafetyReportResults { get; set; }

    public virtual DbSet<SafetyReportStatus> SafetyReportStatuses { get; set; }

    public virtual DbSet<ScriptExecuteTask> ScriptExecuteTasks { get; set; }

    public virtual DbSet<SendEmailTask> SendEmailTasks { get; set; }

    public virtual DbSet<SendSmsTask> SendSmsTasks { get; set; }

    public virtual DbSet<SentSmse> SentSmses { get; set; }

    public virtual DbSet<Sentence> Sentences { get; set; }

    public virtual DbSet<SentenceCancellation> SentenceCancellations { get; set; }

    public virtual DbSet<SentenceStatus> SentenceStatuses { get; set; }

    public virtual DbSet<SeparationExchangeFile> SeparationExchangeFiles { get; set; }

    public virtual DbSet<SeparationInvolvedMember> SeparationInvolvedMembers { get; set; }

    public virtual DbSet<SeparationPlanRequest> SeparationPlanRequests { get; set; }

    public virtual DbSet<SeparationPlanRequestStatus> SeparationPlanRequestStatuses { get; set; }

    public virtual DbSet<SeparationPlanRequestType> SeparationPlanRequestTypes { get; set; }

    public virtual DbSet<SeparationRequestSurveie> SeparationRequestSurveies { get; set; }

    public virtual DbSet<ServiceField> ServiceFields { get; set; }

    public virtual DbSet<ServiceRequest> ServiceRequests { get; set; }

    public virtual DbSet<ServiceType> ServiceTypes { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    public virtual DbSet<Sgaletter> Sgaletters { get; set; }

    public virtual DbSet<SgaletterType> SgaletterTypes { get; set; }

    public virtual DbSet<Signatory> Signatories { get; set; }

    public virtual DbSet<SmsAccount> SmsAccounts { get; set; }

    public virtual DbSet<SolutionResource> SolutionResources { get; set; }

    public virtual DbSet<SqlServerDataSourceConfig> SqlServerDataSourceConfigs { get; set; }

    public virtual DbSet<StepOwnerPool> StepOwnerPools { get; set; }

    public virtual DbSet<StepPending> StepPendings { get; set; }

    public virtual DbSet<StepRecordDataLog> StepRecordDataLogs { get; set; }

    public virtual DbSet<StepRecordLog> StepRecordLogs { get; set; }

    public virtual DbSet<StructureExecutiveOperationsForm> StructureExecutiveOperationsForms { get; set; }

    public virtual DbSet<StructurePlanControlForm> StructurePlanControlForms { get; set; }

    public virtual DbSet<StructureRedLinesForm> StructureRedLinesForms { get; set; }

    public virtual DbSet<StructureSafetyForm> StructureSafetyForms { get; set; }

    public virtual DbSet<StructureSpecForm> StructureSpecForms { get; set; }

    public virtual DbSet<StructureType> StructureTypes { get; set; }

    public virtual DbSet<StudyGrade> StudyGrades { get; set; }

    public virtual DbSet<StudyRecord> StudyRecords { get; set; }

    public virtual DbSet<SubWorkItemSetting> SubWorkItemSettings { get; set; }

    public virtual DbSet<SupervisionCandidate> SupervisionCandidates { get; set; }

    public virtual DbSet<SupervisionCandidateStatus> SupervisionCandidateStatuses { get; set; }

    public virtual DbSet<SupervisionField> SupervisionFields { get; set; }

    public virtual DbSet<SupervisionItemAnswer> SupervisionItemAnswers { get; set; }

    public virtual DbSet<SupervisionOrder> SupervisionOrders { get; set; }

    public virtual DbSet<SupervisionOrderStatus> SupervisionOrderStatuses { get; set; }

    public virtual DbSet<SupervisionOrderType> SupervisionOrderTypes { get; set; }

    public virtual DbSet<SupervisionSetting> SupervisionSettings { get; set; }

    public virtual DbSet<SupervisionStep> SupervisionSteps { get; set; }

    public virtual DbSet<SupervisionStepForm> SupervisionStepForms { get; set; }

    public virtual DbSet<SupervisionStepItem> SupervisionStepItems { get; set; }

    public virtual DbSet<SupervisionStepItemGroup> SupervisionStepItemGroups { get; set; }

    public virtual DbSet<SupervisionStepItemOption> SupervisionStepItemOptions { get; set; }

    public virtual DbSet<SupervisionStepPayment> SupervisionStepPayments { get; set; }

    public virtual DbSet<SupervisionStepPaymentStatus> SupervisionStepPaymentStatuses { get; set; }

    public virtual DbSet<SupportTicket> SupportTickets { get; set; }

    public virtual DbSet<SupportedCulture> SupportedCultures { get; set; }

    public virtual DbSet<SystemSetting> SystemSettings { get; set; }

    public virtual DbSet<TakenUserAction> TakenUserActions { get; set; }

    public virtual DbSet<TaskType> TaskTypes { get; set; }

    public virtual DbSet<TeacherLesson> TeacherLessons { get; set; }

    public virtual DbSet<Temp> Temps { get; set; }

    public virtual DbSet<TempInvolvedMember> TempInvolvedMembers { get; set; }

    public virtual DbSet<Territory> Territories { get; set; }

    public virtual DbSet<TerritoryType> TerritoryTypes { get; set; }

    public virtual DbSet<TicketReply> TicketReplies { get; set; }

    public virtual DbSet<TicketStatus> TicketStatuses { get; set; }

    public virtual DbSet<TimerEvent> TimerEvents { get; set; }

    public virtual DbSet<TimerTick> TimerTicks { get; set; }

    public virtual DbSet<TimerType> TimerTypes { get; set; }

    public virtual DbSet<TrackCode> TrackCodes { get; set; }

    public virtual DbSet<TwoFactorMode> TwoFactorModes { get; set; }

    public virtual DbSet<University> Universities { get; set; }

    public virtual DbSet<UsageType> UsageTypes { get; set; }

    public virtual DbSet<UserActionTask> UserActionTasks { get; set; }

    public virtual DbSet<UserActionTaskAction> UserActionTaskActions { get; set; }

    public virtual DbSet<UserDelegation> UserDelegations { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    public virtual DbSet<UserTypePhoneNumber> UserTypePhoneNumbers { get; set; }

    public virtual DbSet<ValidMemberService> ValidMemberServices { get; set; }

    public virtual DbSet<ViewAction> ViewActions { get; set; }

    public virtual DbSet<ViewComponent> ViewComponents { get; set; }

    public virtual DbSet<ViewComponentView> ViewComponentViews { get; set; }

    public virtual DbSet<ViewType> ViewTypes { get; set; }

    public virtual DbSet<ViolationReport> ViolationReports { get; set; }

    public virtual DbSet<ViolationReportDetail> ViolationReportDetails { get; set; }

    public virtual DbSet<ViolationReportSubjectType> ViolationReportSubjectTypes { get; set; }

    public virtual DbSet<ViolationReportUser> ViolationReportUsers { get; set; }

    public virtual DbSet<VwBillDetail> VwBillDetails { get; set; }

    public virtual DbSet<VwBuildingGroupSetting> VwBuildingGroupSettings { get; set; }

    public virtual DbSet<VwCapacity> VwCapacities { get; set; }

    public virtual DbSet<VwCapacityCalculationsB200> VwCapacityCalculationsB200s { get; set; }

    public virtual DbSet<VwCapacityDetail> VwCapacityDetails { get; set; }

    public virtual DbSet<VwConstructionLisenceLab> VwConstructionLisenceLabs { get; set; }

    public virtual DbSet<VwContractDossier> VwContractDossiers { get; set; }

    public virtual DbSet<VwHistoryCapacity> VwHistoryCapacities { get; set; }

    public virtual DbSet<VwIncompleteBill> VwIncompleteBills { get; set; }

    public virtual DbSet<VwInvolvedMember> VwInvolvedMembers { get; set; }

    public virtual DbSet<VwLicenseInquery> VwLicenseInqueries { get; set; }

    public virtual DbSet<VwMemberCapacityDetail> VwMemberCapacityDetails { get; set; }

    public virtual DbSet<VwNeedExecuterDossier> VwNeedExecuterDossiers { get; set; }

    public virtual DbSet<VwNotReleased> VwNotReleaseds { get; set; }

    public virtual DbSet<VwOperator> VwOperators { get; set; }

    public virtual DbSet<VwPaymentCallBackReport> VwPaymentCallBackReports { get; set; }

    public virtual DbSet<VwPendingStep> VwPendingSteps { get; set; }

    public virtual DbSet<VwPendingTimer> VwPendingTimers { get; set; }

    public virtual DbSet<VwPlanControl> VwPlanControls { get; set; }

    public virtual DbSet<VwSeperationPlan> VwSeperationPlans { get; set; }

    public virtual DbSet<VwStepFom> VwStepFoms { get; set; }

    public virtual DbSet<VwSupervisorContractTempDossier> VwSupervisorContractTempDossiers { get; set; }

    public virtual DbSet<VwUserDeligation> VwUserDeligations { get; set; }

    public virtual DbSet<VwWorkItemsSync> VwWorkItemsSyncs { get; set; }

    public virtual DbSet<WebPushSetting> WebPushSettings { get; set; }

    public virtual DbSet<WebPushSubscription> WebPushSubscriptions { get; set; }

    public virtual DbSet<WebService> WebServices { get; set; }

    public virtual DbSet<WebServiceMethod> WebServiceMethods { get; set; }

    public virtual DbSet<WebServiceMethodRequestHeader> WebServiceMethodRequestHeaders { get; set; }

    public virtual DbSet<WebServiceMethodRouteArgument> WebServiceMethodRouteArguments { get; set; }

    public virtual DbSet<WebServiceMethodType> WebServiceMethodTypes { get; set; }

    public virtual DbSet<WebServiceMethodUrlArgument> WebServiceMethodUrlArguments { get; set; }

    public virtual DbSet<WebServiceType> WebServiceTypes { get; set; }

    public virtual DbSet<WorkItemSetting> WorkItemSettings { get; set; }

    public virtual DbSet<WorkingStatus> WorkingStatuses { get; set; }

    public virtual DbSet<Xpayment> Xpayments { get; set; }

    public virtual DbSet<Zzzzzzzzzz> Zzzzzzzzzzs { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AcceptedPlanControl>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("AcceptedPlanControls");

            entity.Property(e => e.DossierNumber).HasMaxLength(64);
            entity.Property(e => e.FirstName).HasMaxLength(128);
            entity.Property(e => e.LastName).HasMaxLength(128);
        });

        modelBuilder.Entity<ActivityLicense>(entity =>
        {
            entity.ToTable("ActivityLicenses", "cedo");

            entity.HasIndex(e => e.ExpireDate, "IX_ActivityLicenses_ExpireDate_AB28B");

            entity.HasIndex(e => e.IssuedCityId, "IX_ActivityLicenses_IssuedCityId");

            entity.HasIndex(e => e.MemberId, "IX_ActivityLicenses_MemberId");

            entity.Property(e => e.LicenseSerial).HasMaxLength(32);

            entity.HasOne(d => d.IssuedCity).WithMany(p => p.ActivityLicenses)
                .HasForeignKey(d => d.IssuedCityId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Member).WithMany(p => p.ActivityLicenses).HasForeignKey(d => d.MemberId);
        });

        modelBuilder.Entity<ActivityTransferRequest>(entity =>
        {
            entity.ToTable("ActivityTransferRequests", "cedo");

            entity.HasIndex(e => e.CityId, "IX_ActivityTransferRequests_CityId");

            entity.HasIndex(e => e.MemberId, "IX_ActivityTransferRequests_MemberId");

            entity.HasIndex(e => e.StatusId, "IX_ActivityTransferRequests_StatusId");

            entity.Property(e => e.Description).HasMaxLength(1024);
            entity.Property(e => e.DestinationAddress).HasMaxLength(128);
            entity.Property(e => e.DestinationTell).HasMaxLength(32);
            entity.Property(e => e.Workplace).HasMaxLength(128);
            entity.Property(e => e.WorkplaceTell).HasMaxLength(32);

            entity.HasOne(d => d.City).WithMany(p => p.ActivityTransferRequests).HasForeignKey(d => d.CityId);

            entity.HasOne(d => d.Member).WithMany(p => p.ActivityTransferRequests).HasForeignKey(d => d.MemberId);

            entity.HasOne(d => d.Status).WithMany(p => p.ActivityTransferRequests).HasForeignKey(d => d.StatusId);
        });

        modelBuilder.Entity<ActivityTransferRequestStatus>(entity =>
        {
            entity.ToTable("ActivityTransferRequestStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<AdditionalSupervisionField>(entity =>
        {
            entity.ToTable("AdditionalSupervisionFields", "cedo");

            entity.HasIndex(e => e.FieldId, "IX_AdditionalSupervisionFields_FieldId");

            entity.HasIndex(e => e.SupervisionFieldId, "IX_AdditionalSupervisionFields_SupervisionFieldId");

            entity.HasOne(d => d.Field).WithMany(p => p.AdditionalSupervisionFields)
                .HasForeignKey(d => d.FieldId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.SupervisionField).WithMany(p => p.AdditionalSupervisionFields).HasForeignKey(d => d.SupervisionFieldId);
        });

        modelBuilder.Entity<ArchitectureExecutiveOperationsForm>(entity =>
        {
            entity.ToTable("ArchitectureExecutiveOperationsForms", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_ArchitectureExecutiveOperationsForms_InvolvedMemberId");

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.ArchitectureExecutiveOperationsForms)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ArchitecturePlanControlForm>(entity =>
        {
            entity.ToTable("ArchitecturePlanControlForms", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_ArchitecturePlanControlForms_InvolvedMemberId");

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.ArchitecturePlanControlForms)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ArchitectureRedLinesForm>(entity =>
        {
            entity.ToTable("ArchitectureRedLinesForms", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_ArchitectureRedLinesForms_InvolvedMemberId");

            entity.Property(e => e.Description).HasMaxLength(1024);

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.ArchitectureRedLinesForms)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ArchitectureSafetyForm>(entity =>
        {
            entity.ToTable("ArchitectureSafetyForms", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_ArchitectureSafetyForms_ConstructionLicenseId");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_ArchitectureSafetyForms_InvolvedMemberId");

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.ArchitectureSafetyForms).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.ArchitectureSafetyForms).HasForeignKey(d => d.InvolvedMemberId);
        });

        modelBuilder.Entity<ArchitectureSpecForm>(entity =>
        {
            entity.ToTable("ArchitectureSpecForms", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_ArchitectureSpecForms_InvolvedMemberId");

            entity.Property(e => e.ArchitectureQuality).HasDefaultValue(true);
            entity.Property(e => e.CeilingHeatInsulationPlace).HasMaxLength(128);
            entity.Property(e => e.ExteriorWallHeatInsulationPlace).HasMaxLength(128);
            entity.Property(e => e.ExteriorWallsWaterproofingPlace).HasMaxLength(128);
            entity.Property(e => e.FloorHeatInsulationPlace).HasMaxLength(128);
            entity.Property(e => e.OtherExteriorViews).HasMaxLength(128);
            entity.Property(e => e.OtherExteriorWallMaterials).HasMaxLength(128);
            entity.Property(e => e.OtherFalseCeilings).HasMaxLength(128);
            entity.Property(e => e.OtherFloorCovers).HasMaxLength(128);
            entity.Property(e => e.OtherGlassTypes).HasMaxLength(128);
            entity.Property(e => e.OtherInteriorViews).HasMaxLength(128);
            entity.Property(e => e.OtherInteriorWallMaterials).HasMaxLength(128);
            entity.Property(e => e.OtherRoofTopCovers).HasMaxLength(128);
            entity.Property(e => e.OtherStairs).HasMaxLength(128);
            entity.Property(e => e.OtherWindows).HasMaxLength(128);
            entity.Property(e => e.StandardArchitectureMaterials).HasDefaultValue(true);

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.ArchitectureSpecForms)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<AsBuiltRequest>(entity =>
        {
            entity.ToTable("AsBuiltRequest", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_AsBuiltRequest_ConstructionLicenseId");

            entity.HasIndex(e => e.FinalBuildingGroupId, "IX_AsBuiltRequest_FinalBuildingGroupId");

            entity.HasIndex(e => e.IntialBuildingGroupSettingId, "IX_AsBuiltRequest_IntialBuildingGroupSettingId");

            entity.HasIndex(e => e.StatusIdId, "IX_AsBuiltRequest_StatusIdId");

            entity.Property(e => e.Description).HasMaxLength(1024);

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.AsBuiltRequests).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.FinalBuildingGroup).WithMany(p => p.AsBuiltRequestFinalBuildingGroups).HasForeignKey(d => d.FinalBuildingGroupId);

            entity.HasOne(d => d.IntialBuildingGroupSetting).WithMany(p => p.AsBuiltRequestIntialBuildingGroupSettings).HasForeignKey(d => d.IntialBuildingGroupSettingId);

            entity.HasOne(d => d.StatusId).WithMany(p => p.AsBuiltRequests).HasForeignKey(d => d.StatusIdId);
        });

        modelBuilder.Entity<AsBuiltRequestStatus>(entity =>
        {
            entity.ToTable("AsBuiltRequestStatus", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<Attendee>(entity =>
        {
            entity.ToTable("Attendees", "cedo");

            entity.HasIndex(e => e.CourseHoldingMemberId, "IX_Attendees_CourseHoldingMemberId");

            entity.HasIndex(e => e.HoldingLessonId, "IX_Attendees_HoldingLessonId");

            entity.HasIndex(e => e.HoldingLessonId, "IX_Attendees_HoldingLessonId_CCC9A");

            entity.HasIndex(e => new { e.HoldingLessonId, e.StatusId }, "IX_Attendees_HoldingLessonId_StatusId_CC33F");

            entity.HasIndex(e => new { e.HoldingLessonId, e.StatusId, e.Score }, "IX_Attendees_HoldingLessonId_StatusId_Score_48338");

            entity.HasIndex(e => e.StatusId, "IX_Attendees_StatusId");

            entity.HasIndex(e => e.StatusId, "IX_Attendees_StatusId_46729");

            entity.HasIndex(e => e.StatusId, "IX_Attendees_StatusId_6AEA2");

            entity.HasIndex(e => new { e.StatusId, e.Score }, "IX_Attendees_StatusId_Score_F3F27");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Test).HasColumnName("test");

            entity.HasOne(d => d.CourseHoldingMember).WithMany(p => p.Attendees).HasForeignKey(d => d.CourseHoldingMemberId);

            entity.HasOne(d => d.HoldingLesson).WithMany(p => p.Attendees).HasForeignKey(d => d.HoldingLessonId);

            entity.HasOne(d => d.Status).WithMany(p => p.Attendees).HasForeignKey(d => d.StatusId);
        });

        modelBuilder.Entity<AttendeeOption>(entity =>
        {
            entity.ToTable("AttendeeOptions", "cedo");

            entity.HasIndex(e => e.AttendeeId, "IX_AttendeeOptions_AttendeeId");

            entity.HasIndex(e => e.OptionId, "IX_AttendeeOptions_OptionId");

            entity.HasOne(d => d.Attendee).WithMany(p => p.AttendeeOptions).HasForeignKey(d => d.AttendeeId);

            entity.HasOne(d => d.Option).WithMany(p => p.AttendeeOptions)
                .HasForeignKey(d => d.OptionId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<AttendeeSession>(entity =>
        {
            entity.ToTable("AttendeeSessions", "cedo");

            entity.HasIndex(e => e.AttendeeId, "IX_AttendeeSessions_AttendeeId");

            entity.HasIndex(e => e.SessionId, "IX_AttendeeSessions_SessionId");

            entity.HasIndex(e => e.StatusId, "IX_AttendeeSessions_StatusId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Attendee).WithMany(p => p.AttendeeSessions).HasForeignKey(d => d.AttendeeId);

            entity.HasOne(d => d.Session).WithMany(p => p.AttendeeSessions).HasForeignKey(d => d.SessionId);

            entity.HasOne(d => d.Status).WithMany(p => p.AttendeeSessions).HasForeignKey(d => d.StatusId);
        });

        modelBuilder.Entity<AttendeeSessionStatus>(entity =>
        {
            entity.ToTable("AttendeeSessionStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<AttendeeStatus>(entity =>
        {
            entity.ToTable("AttendeeStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<AttributePropertyValue>(entity =>
        {
            entity.HasIndex(e => e.PropertyAttributeId, "IX_AttributePropertyValues_PropertyAttributeId");

            entity.Property(e => e.PropertyName).HasMaxLength(128);
            entity.Property(e => e.PropertyValue).HasMaxLength(128);

            entity.HasOne(d => d.PropertyAttribute).WithMany(p => p.AttributePropertyValues).HasForeignKey(d => d.PropertyAttributeId);
        });

        modelBuilder.Entity<BankAcountType>(entity =>
        {
            entity.ToTable("BankAcountTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<Bill>(entity =>
        {
            entity.ToTable("Bills", "cedo");

            entity.HasIndex(e => e.BillTypeId, "IX_Bills_BillTypeId");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_Bills_ConstructionLicenseId");

            entity.HasIndex(e => e.CourseHoldingMemberId, "IX_Bills_CourseHoldingMemberId");

            entity.HasIndex(e => e.GasRequestId, "IX_Bills_GasRequestId");

            entity.HasIndex(e => e.GasRequestOncallId, "IX_Bills_GasRequestOncallId");

            entity.HasIndex(e => e.SeparationRequestId, "IX_Bills_SeparationRequestId");

            entity.Property(e => e.Description).HasMaxLength(2000);

            entity.HasOne(d => d.BillType).WithMany(p => p.Bills).HasForeignKey(d => d.BillTypeId);

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.Bills).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.CourseHoldingMember).WithMany(p => p.Bills).HasForeignKey(d => d.CourseHoldingMemberId);

            entity.HasOne(d => d.GasRequest).WithMany(p => p.Bills).HasForeignKey(d => d.GasRequestId);

            entity.HasOne(d => d.GasRequestOncall).WithMany(p => p.Bills).HasForeignKey(d => d.GasRequestOncallId);

            entity.HasOne(d => d.SeparationRequest).WithMany(p => p.Bills).HasForeignKey(d => d.SeparationRequestId);
        });

        modelBuilder.Entity<BillDetail>(entity =>
        {
            entity.ToTable("BillDetails", "cedo");

            entity.HasIndex(e => e.BillId, "IX_BillDetails_BillId");

            entity.HasIndex(e => e.CostTypeId, "IX_BillDetails_CostTypeId");

            entity.HasOne(d => d.Bill).WithMany(p => p.BillDetails).HasForeignKey(d => d.BillId);

            entity.HasOne(d => d.CostType).WithMany(p => p.BillDetails).HasForeignKey(d => d.CostTypeId);
        });

        modelBuilder.Entity<BillDetailItem>(entity =>
        {
            entity.ToTable("BillDetailItems", "cedo");

            entity.HasIndex(e => e.BillDetailId, "IX_BillDetailItems_BillDetailId");

            entity.HasIndex(e => e.TariffDetailItemId, "IX_BillDetailItems_TariffDetailItemId");

            entity.HasOne(d => d.BillDetail).WithMany(p => p.BillDetailItems).HasForeignKey(d => d.BillDetailId);

            entity.HasOne(d => d.TariffDetailItem).WithMany(p => p.BillDetailItems)
                .HasForeignKey(d => d.TariffDetailItemId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BillType>(entity =>
        {
            entity.ToTable("BillTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<BoundaryEvent>(entity =>
        {
            entity.HasIndex(e => e.BoundarySourceId, "IX_BoundaryEvents_BoundarySourceId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.BoundarySource).WithMany(p => p.BoundaryEventBoundarySources)
                .HasForeignKey(d => d.BoundarySourceId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.BoundaryEventIdNavigation).HasForeignKey<BoundaryEvent>(d => d.Id);
        });

        modelBuilder.Entity<BuildingCertificate>(entity =>
        {
            entity.ToTable("BuildingCertificates", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_BuildingCertificates_ConstructionLicenseId");

            entity.HasIndex(e => e.ConstructionLicenseTypeId, "IX_BuildingCertificates_ConstructionLicenseTypeId");

            entity.Property(e => e.BuildingCertificateNo).HasMaxLength(128);
            entity.Property(e => e.Description).HasMaxLength(1024);
            entity.Property(e => e.DossierNo).HasMaxLength(128);

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.BuildingCertificates).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.ConstructionLicenseType).WithMany(p => p.BuildingCertificates)
                .HasForeignKey(d => d.ConstructionLicenseTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BuildingCertificateTimeout>(entity =>
        {
            entity.ToTable("BuildingCertificateTimeouts", "cedo");
        });

        modelBuilder.Entity<BuildingGroup>(entity =>
        {
            entity.ToTable("BuildingGroups", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<BuildingGroupSetting>(entity =>
        {
            entity.ToTable("BuildingGroupSettings", "cedo");

            entity.HasIndex(e => e.DossierTypeId, "IX_BuildingGroupSettings_DossierTypeId");

            entity.HasIndex(e => e.IsActive, "IX_BuildingGroupSettings_IsActive_98DE9");

            entity.HasIndex(e => new { e.IsActive, e.DossierTypeId }, "IX_BuildingGroupSettings_IsActive_DossierTypeId_6E35C");

            entity.HasIndex(e => e.IsActive, "IX_BuildingGroupSettings_IsActive_FBA38");

            entity.HasIndex(e => new { e.IsActive, e.SubGroupId }, "IX_BuildingGroupSettings_IsActive_SubGroupId_25215");

            entity.HasIndex(e => e.MunicipalityId, "IX_BuildingGroupSettings_MunicipalityId");

            entity.HasIndex(e => e.SubGroupId, "IX_BuildingGroupSettings_SubGroupId");

            entity.Property(e => e.ArchitechtureContractDuration).HasDefaultValue(1);
            entity.Property(e => e.ElectricityContractDuration).HasDefaultValue(1);
            entity.Property(e => e.MechanicContractDuration).HasDefaultValue(1);
            entity.Property(e => e.MinFloorsNeedsExecuter).HasDefaultValue(5);
            entity.Property(e => e.StructureContractDuration).HasDefaultValue(1);
            entity.Property(e => e.SubGroupId).HasDefaultValue(1);
            entity.Property(e => e.SupervisorCount).HasDefaultValue(5);
            entity.Property(e => e.Title).HasMaxLength(128);

            entity.HasOne(d => d.DossierType).WithMany(p => p.BuildingGroupSettings).HasForeignKey(d => d.DossierTypeId);

            entity.HasOne(d => d.Municipality).WithMany(p => p.BuildingGroupSettings).HasForeignKey(d => d.MunicipalityId);

            entity.HasOne(d => d.SubGroup).WithMany(p => p.BuildingGroupSettings).HasForeignKey(d => d.SubGroupId);
        });

        modelBuilder.Entity<BuildingGroupSettingCity>(entity =>
        {
            entity.ToTable("BuildingGroupSettingCities", "cedo");

            entity.HasIndex(e => e.BuildingGroupSettingId, "IX_BuildingGroupSettingCities_BuildingGroupSettingId");

            entity.HasIndex(e => e.CityId, "IX_BuildingGroupSettingCities_CityId");

            entity.HasOne(d => d.BuildingGroupSetting).WithMany(p => p.BuildingGroupSettingCities).HasForeignKey(d => d.BuildingGroupSettingId);

            entity.HasOne(d => d.City).WithMany(p => p.BuildingGroupSettingCities).HasForeignKey(d => d.CityId);
        });

        modelBuilder.Entity<BuildingGroupSettingRatio>(entity =>
        {
            entity.ToTable("BuildingGroupSettingRatios", "cedo");

            entity.HasIndex(e => e.BuildingGroupSettingId, "IX_BuildingGroupSettingRatios_BuildingGroupSettingId");

            entity.HasIndex(e => e.MemberTypeId, "IX_BuildingGroupSettingRatios_MemberTypeId");

            entity.HasIndex(e => e.ServiceFieldId, "IX_BuildingGroupSettingRatios_ServiceFieldId");

            entity.HasIndex(e => e.ServiceTypeId, "IX_BuildingGroupSettingRatios_ServiceTypeId");

            entity.HasOne(d => d.BuildingGroupSetting).WithMany(p => p.BuildingGroupSettingRatios).HasForeignKey(d => d.BuildingGroupSettingId);

            entity.HasOne(d => d.MemberType).WithMany(p => p.BuildingGroupSettingRatios).HasForeignKey(d => d.MemberTypeId);

            entity.HasOne(d => d.ServiceField).WithMany(p => p.BuildingGroupSettingRatios).HasForeignKey(d => d.ServiceFieldId);

            entity.HasOne(d => d.ServiceType).WithMany(p => p.BuildingGroupSettingRatios).HasForeignKey(d => d.ServiceTypeId);
        });

        modelBuilder.Entity<BuildingSubGroup>(entity =>
        {
            entity.ToTable("BuildingSubGroups", "cedo");

            entity.HasIndex(e => e.BuildingGroupId, "IX_BuildingSubGroups_BuildingGroupId");

            entity.Property(e => e.Title).HasMaxLength(128);

            entity.HasOne(d => d.BuildingGroup).WithMany(p => p.BuildingSubGroups).HasForeignKey(d => d.BuildingGroupId);
        });

        modelBuilder.Entity<BuildingUnit>(entity =>
        {
            entity.ToTable("BuildingUnits", "cedo");

            entity.HasIndex(e => e.FloorId, "IX_BuildingUnits_FloorId");

            entity.HasIndex(e => e.UsageTypeId, "IX_BuildingUnits_UsageTypeId");

            entity.HasOne(d => d.Floor).WithMany(p => p.BuildingUnits).HasForeignKey(d => d.FloorId);

            entity.HasOne(d => d.UsageType).WithMany(p => p.BuildingUnits)
                .HasForeignKey(d => d.UsageTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BuildingWorker>(entity =>
        {
            entity.ToTable("BuildingWorkers", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_BuildingWorkers_InvolvedMemberId");

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.BuildingWorkers)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<CancelDossierBill>(entity =>
        {
            entity.ToTable("CancelDossierBills", "cedo");

            entity.HasIndex(e => e.CancelDossierRequestId, "IX_CancelDossierBills_CancelDossierRequestId");

            entity.Property(e => e.AccountNumber).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(1024);

            entity.HasOne(d => d.CancelDossierRequest).WithMany(p => p.CancelDossierBills).HasForeignKey(d => d.CancelDossierRequestId);
        });

        modelBuilder.Entity<CancelDossierBillDetail>(entity =>
        {
            entity.ToTable("CancelDossierBillDetails", "cedo");

            entity.HasIndex(e => e.CancelDossierBillId, "IX_CancelDossierBillDetails_CancelDossierBillId");

            entity.HasIndex(e => e.CostTypeId, "IX_CancelDossierBillDetails_CostTypeId");

            entity.Property(e => e.Description).HasMaxLength(1024);

            entity.HasOne(d => d.CancelDossierBill).WithMany(p => p.CancelDossierBillDetails).HasForeignKey(d => d.CancelDossierBillId);

            entity.HasOne(d => d.CostType).WithMany(p => p.CancelDossierBillDetails).HasForeignKey(d => d.CostTypeId);
        });

        modelBuilder.Entity<CancelDossierRequest>(entity =>
        {
            entity.ToTable("CancelDossierRequests", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_CancelDossierRequests_ConstructionLicenseId");

            entity.HasIndex(e => e.RequestStatusId, "IX_CancelDossierRequests_RequestStatusId");

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.CancelDossierRequests).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.RequestStatus).WithMany(p => p.CancelDossierRequests).HasForeignKey(d => d.RequestStatusId);
        });

        modelBuilder.Entity<CancelDossierStatus>(entity =>
        {
            entity.ToTable("CancelDossierStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<CancelEvent>(entity =>
        {
            entity.ToTable("CancelEvent");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ConfirmMessage).HasMaxLength(2048);
            entity.Property(e => e.Title).HasMaxLength(256);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.CancelEvent).HasForeignKey<CancelEvent>(d => d.Id);
        });

        modelBuilder.Entity<CapacityCalculation>(entity =>
        {
            entity.ToTable("CapacityCalculations", "cedo");

            entity.HasIndex(e => e.BuildingGroupSettingId, "IX_CapacityCalculations_BuildingGroupSettingId");

            entity.HasIndex(e => e.ServiceFieldId, "IX_CapacityCalculations_ServiceFieldId");

            entity.HasIndex(e => e.ServiceTypeId, "IX_CapacityCalculations_ServiceTypeId");

            entity.Property(e => e.MaxWorkItem).HasDefaultValue(8);

            entity.HasOne(d => d.BuildingGroupSetting).WithMany(p => p.CapacityCalculations).HasForeignKey(d => d.BuildingGroupSettingId);

            entity.HasOne(d => d.ServiceField).WithMany(p => p.CapacityCalculations).HasForeignKey(d => d.ServiceFieldId);

            entity.HasOne(d => d.ServiceType).WithMany(p => p.CapacityCalculations).HasForeignKey(d => d.ServiceTypeId);
        });

        modelBuilder.Entity<CategoryManpowerSkillCard>(entity =>
        {
            entity.ToTable("CategoryManpowerSkillCards", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<CedoAgency>(entity =>
        {
            entity.ToTable("CedoAgencies", "cedo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(512);
            entity.Property(e => e.Fax).HasMaxLength(128);
            entity.Property(e => e.PostalCode).HasMaxLength(128);
            entity.Property(e => e.Telephone).HasMaxLength(128);
            entity.Property(e => e.Title).HasMaxLength(256);
            entity.Property(e => e.WebSite).HasMaxLength(128);
        });

        modelBuilder.Entity<CedoAgencyCity>(entity =>
        {
            entity.ToTable("CedoAgencyCities", "cedo");

            entity.HasIndex(e => e.AgencyId, "IX_CedoAgencyCities_AgencyId");

            entity.HasIndex(e => e.CityId, "IX_CedoAgencyCities_CityId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Agency).WithMany(p => p.CedoAgencyCities).HasForeignKey(d => d.AgencyId);

            entity.HasOne(d => d.City).WithMany(p => p.CedoAgencyCities).HasForeignKey(d => d.CityId);
        });

        modelBuilder.Entity<ChangeDesignerRequest>(entity =>
        {
            entity.ToTable("ChangeDesignerRequests", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_ChangeDesignerRequests_ConstructionLicenseId");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_ChangeDesignerRequests_InvolvedMemberId");

            entity.HasIndex(e => e.RequestId, "IX_ChangeDesignerRequests_RequestId");

            entity.HasIndex(e => e.RequestStatusId, "IX_ChangeDesignerRequests_RequestStatusId");

            entity.HasIndex(e => e.ServiceFieldId, "IX_ChangeDesignerRequests_ServiceFieldId");

            entity.Property(e => e.Description).HasMaxLength(1024);

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.ChangeDesignerRequests).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.ChangeDesignerRequests).HasForeignKey(d => d.InvolvedMemberId);

            entity.HasOne(d => d.Request).WithMany(p => p.ChangeDesignerRequests).HasForeignKey(d => d.RequestId);

            entity.HasOne(d => d.RequestStatus).WithMany(p => p.ChangeDesignerRequests).HasForeignKey(d => d.RequestStatusId);

            entity.HasOne(d => d.ServiceField).WithMany(p => p.ChangeDesignerRequests).HasForeignKey(d => d.ServiceFieldId);
        });

        modelBuilder.Entity<ChangeDesignerRequestStatus>(entity =>
        {
            entity.ToTable("ChangeDesignerRequestStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ChangeExecuterRequest>(entity =>
        {
            entity.ToTable("ChangeExecuterRequest", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_ChangeExecuterRequest_ConstructionLicenseId");

            entity.HasIndex(e => e.RequestId, "IX_ChangeExecuterRequest_RequestId");

            entity.HasIndex(e => e.StatusId, "IX_ChangeExecuterRequest_StatusId");

            entity.Property(e => e.ExecuterDescription).HasMaxLength(2048);
            entity.Property(e => e.OwnerDescription).HasMaxLength(2048);

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.ChangeExecuterRequests).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.Request).WithMany(p => p.ChangeExecuterRequests).HasForeignKey(d => d.RequestId);

            entity.HasOne(d => d.Status).WithMany(p => p.ChangeExecuterRequests).HasForeignKey(d => d.StatusId);
        });

        modelBuilder.Entity<ChangeExecuterRequestStatus>(entity =>
        {
            entity.ToTable("ChangeExecuterRequestStatus", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ChangeOwnerRequest>(entity =>
        {
            entity.ToTable("ChangeOwnerRequests", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_ChangeOwnerRequests_ConstructionLicenseId");

            entity.HasIndex(e => e.RequestId, "IX_ChangeOwnerRequests_RequestId");

            entity.HasIndex(e => e.RequestStatusId, "IX_ChangeOwnerRequests_RequestStatusId");

            entity.Property(e => e.BuildingProgress).HasDefaultValue(0);
            entity.Property(e => e.LicenseNumber).HasMaxLength(64);
            entity.Property(e => e.RefNumber).HasMaxLength(64);

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.ChangeOwnerRequests).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.Request).WithMany(p => p.ChangeOwnerRequests).HasForeignKey(d => d.RequestId);

            entity.HasOne(d => d.RequestStatus).WithMany(p => p.ChangeOwnerRequests).HasForeignKey(d => d.RequestStatusId);
        });

        modelBuilder.Entity<ChangeOwnerRequestDetail>(entity =>
        {
            entity.ToTable("ChangeOwnerRequestDetails", "cedo");

            entity.HasIndex(e => e.ChangeOwnerRequestId, "IX_ChangeOwnerRequestDetails_ChangeOwnerRequestId");

            entity.HasIndex(e => e.OwnerId, "IX_ChangeOwnerRequestDetails_OwnerId");

            entity.HasOne(d => d.ChangeOwnerRequest).WithMany(p => p.ChangeOwnerRequestDetails).HasForeignKey(d => d.ChangeOwnerRequestId);

            entity.HasOne(d => d.Owner).WithMany(p => p.ChangeOwnerRequestDetails)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ChangeOwnerStatus>(entity =>
        {
            entity.ToTable("ChangeOwnerStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ChangePlanFeasibility>(entity =>
        {
            entity.ToTable("ChangePlanFeasibility", "cedo");

            entity.HasIndex(e => e.ChangePlanRequestId, "IX_ChangePlanFeasibility_ChangePlanRequestId");

            entity.Property(e => e.Description).HasMaxLength(2048);

            entity.HasOne(d => d.ChangePlanRequest).WithMany(p => p.ChangePlanFeasibilities).HasForeignKey(d => d.ChangePlanRequestId);
        });

        modelBuilder.Entity<ChangePlanFeasibilityJudgement>(entity =>
        {
            entity.ToTable("ChangePlanFeasibilityJudgement", "cedo");

            entity.HasIndex(e => e.ChangePlanFeasibilityId, "IX_ChangePlanFeasibilityJudgement_ChangePlanFeasibilityId");

            entity.HasIndex(e => e.MemberServiceId, "IX_ChangePlanFeasibilityJudgement_MemberServiceId");

            entity.HasIndex(e => e.ResultFileId, "IX_ChangePlanFeasibilityJudgement_ResultFileId");

            entity.HasOne(d => d.ChangePlanFeasibility).WithMany(p => p.ChangePlanFeasibilityJudgements).HasForeignKey(d => d.ChangePlanFeasibilityId);

            entity.HasOne(d => d.MemberService).WithMany(p => p.ChangePlanFeasibilityJudgements).HasForeignKey(d => d.MemberServiceId);

            entity.HasOne(d => d.ResultFile).WithMany(p => p.ChangePlanFeasibilityJudgements).HasForeignKey(d => d.ResultFileId);
        });

        modelBuilder.Entity<ChangePlanRequest>(entity =>
        {
            entity.ToTable("ChangePlanRequests", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_ChangePlanRequests_ConstructionLicenseId");

            entity.HasIndex(e => e.MemberServiceId, "IX_ChangePlanRequests_MemberServiceId");

            entity.HasIndex(e => e.RequestId, "IX_ChangePlanRequests_RequestId");

            entity.HasIndex(e => e.StatusId, "IX_ChangePlanRequests_StatusId");

            entity.Property(e => e.Description).HasMaxLength(2048);
            entity.Property(e => e.TempDossierNumber).HasMaxLength(128);

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.ChangePlanRequests).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.MemberService).WithMany(p => p.ChangePlanRequests)
                .HasForeignKey(d => d.MemberServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Request).WithMany(p => p.ChangePlanRequests).HasForeignKey(d => d.RequestId);

            entity.HasOne(d => d.Status).WithMany(p => p.ChangePlanRequests).HasForeignKey(d => d.StatusId);
        });

        modelBuilder.Entity<ChangePlanRequestStatus>(entity =>
        {
            entity.ToTable("ChangePlanRequestStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ChangeSiteManagerRequest>(entity =>
        {
            entity.ToTable("ChangeSiteManagerRequests", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_ChangeSiteManagerRequests_ConstructionLicenseId");

            entity.HasIndex(e => e.RequesterId, "IX_ChangeSiteManagerRequests_RequesterId");

            entity.HasIndex(e => e.StatusId, "IX_ChangeSiteManagerRequests_StatusId");

            entity.Property(e => e.Description).HasMaxLength(2000);

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.ChangeSiteManagerRequests).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.Requester).WithMany(p => p.ChangeSiteManagerRequests).HasForeignKey(d => d.RequesterId);

            entity.HasOne(d => d.Status).WithMany(p => p.ChangeSiteManagerRequests).HasForeignKey(d => d.StatusId);
        });

        modelBuilder.Entity<ChangeSiteManagerStatus>(entity =>
        {
            entity.ToTable("ChangeSiteManagerStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ChangeSupervisorJudgment>(entity =>
        {
            entity.ToTable("ChangeSupervisorJudgment", "cedo");

            entity.HasIndex(e => e.ChangeSupervisorRequestId, "IX_ChangeSupervisorJudgment_ChangeSupervisorRequestId");

            entity.HasIndex(e => e.MemberServiceId, "IX_ChangeSupervisorJudgment_MemberServiceId");

            entity.HasIndex(e => e.ResultFileId, "IX_ChangeSupervisorJudgment_ResultFileId");

            entity.HasOne(d => d.ChangeSupervisorRequest).WithMany(p => p.ChangeSupervisorJudgments).HasForeignKey(d => d.ChangeSupervisorRequestId);

            entity.HasOne(d => d.MemberService).WithMany(p => p.ChangeSupervisorJudgments).HasForeignKey(d => d.MemberServiceId);

            entity.HasOne(d => d.ResultFile).WithMany(p => p.ChangeSupervisorJudgments).HasForeignKey(d => d.ResultFileId);
        });

        modelBuilder.Entity<ChangeSupervisorRequest>(entity =>
        {
            entity.ToTable("ChangeSupervisorRequest", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_ChangeSupervisorRequest_ConstructionLicenseId");

            entity.HasIndex(e => e.NewSupervisroMemebrServiceId, "IX_ChangeSupervisorRequest_NewSupervisroMemebrServiceId");

            entity.HasIndex(e => e.RequestId, "IX_ChangeSupervisorRequest_RequestId");

            entity.HasIndex(e => e.StatusId, "IX_ChangeSupervisorRequest_StatusId");

            entity.HasIndex(e => e.SupervisroMemebrServiceId, "IX_ChangeSupervisorRequest_SupervisroMemebrServiceId");

            entity.Property(e => e.ArbitrationDescription).HasMaxLength(2048);
            entity.Property(e => e.EngineeringServiceDescription).HasMaxLength(2048);
            entity.Property(e => e.EngineeringServiceManagerDescription).HasMaxLength(2048);
            entity.Property(e => e.OwnerDescription).HasMaxLength(2048);
            entity.Property(e => e.SupervisorDescription).HasMaxLength(2048);

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.ChangeSupervisorRequests).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.NewSupervisroMemebrService).WithMany(p => p.ChangeSupervisorRequestNewSupervisroMemebrServices).HasForeignKey(d => d.NewSupervisroMemebrServiceId);

            entity.HasOne(d => d.Request).WithMany(p => p.ChangeSupervisorRequests).HasForeignKey(d => d.RequestId);

            entity.HasOne(d => d.Status).WithMany(p => p.ChangeSupervisorRequests).HasForeignKey(d => d.StatusId);

            entity.HasOne(d => d.SupervisroMemebrService).WithMany(p => p.ChangeSupervisorRequestSupervisroMemebrServices).HasForeignKey(d => d.SupervisroMemebrServiceId);
        });

        modelBuilder.Entity<ChangeSupervisorRequestStatus>(entity =>
        {
            entity.ToTable("ChangeSupervisorRequestStatus", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<CheckUser>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("checkUser");

            entity.Property(e => e.Father).HasMaxLength(128);
            entity.Property(e => e.FirstName).HasMaxLength(128);
            entity.Property(e => e.LastName).HasMaxLength(128);
            entity.Property(e => e.MembershipCode).HasMaxLength(32);
            entity.Property(e => e.NationalCode).HasMaxLength(10);
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("Cities", "cedo");

            entity.HasIndex(e => e.ProvinceId, "IX_Cities_ProvinceId");

            entity.Property(e => e.IsSatelliteConfig).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.Province).WithMany(p => p.Cities).HasForeignKey(d => d.ProvinceId);
        });

        modelBuilder.Entity<Complainee>(entity =>
        {
            entity.ToTable("Complainees", "cedo");

            entity.HasIndex(e => e.ComplaintId, "IX_Complainees_ComplaintId");

            entity.Property(e => e.Address).HasMaxLength(1024);
            entity.Property(e => e.FirstName).HasMaxLength(128);
            entity.Property(e => e.LastName).HasMaxLength(128);
            entity.Property(e => e.NationalCode).HasMaxLength(10);

            entity.HasOne(d => d.Complaint).WithMany(p => p.Complainees).HasForeignKey(d => d.ComplaintId);
        });

        modelBuilder.Entity<Complaint>(entity =>
        {
            entity.ToTable("Complaints", "cedo");

            entity.HasIndex(e => e.ComplaintStatusId, "IX_Complaints_ComplaintStatusId");

            entity.Property(e => e.ComplainerAddress)
                .HasMaxLength(1024)
                .HasDefaultValue("");
            entity.Property(e => e.ComplainerZipCode)
                .HasMaxLength(10)
                .HasDefaultValue("");
            entity.Property(e => e.Title).HasMaxLength(512);
            entity.Property(e => e.ViolationPlace)
                .HasMaxLength(1024)
                .HasDefaultValue("");

            entity.HasOne(d => d.ComplaintStatus).WithMany(p => p.Complaints).HasForeignKey(d => d.ComplaintStatusId);
        });

        modelBuilder.Entity<ComplaintCommunicated>(entity =>
        {
            entity.ToTable("ComplaintCommunicateds", "cedo");

            entity.HasIndex(e => e.ComplaintId, "IX_ComplaintCommunicateds_ComplaintId");

            entity.HasIndex(e => e.ComplaintResultStatusId, "IX_ComplaintCommunicateds_ComplaintResultStatusId");

            entity.Property(e => e.Subject)
                .HasMaxLength(512)
                .HasDefaultValue("");

            entity.HasOne(d => d.Complaint).WithMany(p => p.ComplaintCommunicateds).HasForeignKey(d => d.ComplaintId);

            entity.HasOne(d => d.ComplaintResultStatus).WithMany(p => p.ComplaintCommunicateds).HasForeignKey(d => d.ComplaintResultStatusId);
        });

        modelBuilder.Entity<ComplaintMember>(entity =>
        {
            entity.ToTable("ComplaintMembers", "cedo");

            entity.HasIndex(e => e.ComplaintId, "IX_ComplaintMembers_ComplaintId");

            entity.HasIndex(e => e.ComplaintMemberTypeId, "IX_ComplaintMembers_ComplaintMemberTypeId");

            entity.HasIndex(e => e.UserId, "IX_ComplaintMembers_UserId");

            entity.HasOne(d => d.Complaint).WithMany(p => p.ComplaintMembers).HasForeignKey(d => d.ComplaintId);

            entity.HasOne(d => d.ComplaintMemberType).WithMany(p => p.ComplaintMembers).HasForeignKey(d => d.ComplaintMemberTypeId);

            entity.HasOne(d => d.User).WithMany(p => p.ComplaintMembers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ComplaintMemberType>(entity =>
        {
            entity.ToTable("ComplaintMemberTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ComplaintResultMember>(entity =>
        {
            entity.ToTable("ComplaintResultMembers", "cedo");

            entity.HasIndex(e => e.ComplaintMemberId, "IX_ComplaintResultMembers_ComplaintMemberId");

            entity.HasIndex(e => e.ComplaintResultId, "IX_ComplaintResultMembers_ComplaintResultId");

            entity.HasOne(d => d.ComplaintMember).WithMany(p => p.ComplaintResultMembers).HasForeignKey(d => d.ComplaintMemberId);

            entity.HasOne(d => d.ComplaintResult).WithMany(p => p.ComplaintResultMembers)
                .HasForeignKey(d => d.ComplaintResultId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ComplaintResultStatus>(entity =>
        {
            entity.ToTable("ComplaintResultStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ComplaintReview>(entity =>
        {
            entity.ToTable("ComplaintReviews", "cedo");

            entity.HasIndex(e => e.ApplicantId, "IX_ComplaintReviews_ApplicantId");

            entity.HasIndex(e => e.ComplaintId, "IX_ComplaintReviews_ComplaintId");

            entity.HasIndex(e => e.ComplaintReviewTypeId, "IX_ComplaintReviews_ComplaintReviewTypeId");

            entity.HasOne(d => d.Applicant).WithMany(p => p.ComplaintReviews)
                .HasForeignKey(d => d.ApplicantId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Complaint).WithMany(p => p.ComplaintReviews).HasForeignKey(d => d.ComplaintId);

            entity.HasOne(d => d.ComplaintReviewType).WithMany(p => p.ComplaintReviews).HasForeignKey(d => d.ComplaintReviewTypeId);
        });

        modelBuilder.Entity<ComplaintReviewType>(entity =>
        {
            entity.ToTable("ComplaintReviewTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ComplaintStatus>(entity =>
        {
            entity.ToTable("ComplaintStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ConstructionLicense>(entity =>
        {
            entity.ToTable("ConstructionLicenses", "cedo");

            entity.HasIndex(e => e.BuildingGroupSettingId, "IX_ConstructionLicenses_BuildingGroupSettingId");

            entity.HasIndex(e => e.ConstructionLicenseTypeId, "IX_ConstructionLicenses_ConstructionLicenseTypeId");

            entity.HasIndex(e => e.DeploymentSideId, "IX_ConstructionLicenses_DeploymentSideId");

            entity.HasIndex(e => e.DossierTypeId, "IX_ConstructionLicenses_DossierTypeId");

            entity.HasIndex(e => e.EstateId, "IX_ConstructionLicenses_EstateId");

            entity.HasIndex(e => e.ExcavationDangerStatusId, "IX_ConstructionLicenses_ExcavationDangerStatusId");

            entity.HasIndex(e => e.FinalBuildingGroupSettingId, "IX_ConstructionLicenses_FinalBuildingGroupSettingId");

            entity.HasIndex(e => new { e.LicenseNumber, e.StatusId }, "IX_ConstructionLicenses_LicenseNumber_StatusId_6B692");

            entity.HasIndex(e => e.ParentId, "IX_ConstructionLicenses_ParentId");

            entity.HasIndex(e => e.StatusId, "IX_ConstructionLicenses_StatusId");

            entity.HasIndex(e => e.StepId, "IX_ConstructionLicenses_StepId");

            entity.HasIndex(e => e.StructureTypeId, "IX_ConstructionLicenses_StructureTypeId");

            entity.HasIndex(e => e.UsageTypeId, "IX_ConstructionLicenses_UsageTypeId");

            entity.Property(e => e.DossierNumber).HasMaxLength(64);
            entity.Property(e => e.DossierSerial).HasMaxLength(50);
            entity.Property(e => e.DossierTypeId).HasDefaultValue(1);
            entity.Property(e => e.LicenseNumber).HasMaxLength(64);
            entity.Property(e => e.OtherUsageTypes).HasMaxLength(256);
            entity.Property(e => e.RefNumber).HasMaxLength(64);

            entity.HasOne(d => d.BuildingGroupSetting).WithMany(p => p.ConstructionLicenseBuildingGroupSettings).HasForeignKey(d => d.BuildingGroupSettingId);

            entity.HasOne(d => d.ConstructionLicenseType).WithMany(p => p.ConstructionLicenses).HasForeignKey(d => d.ConstructionLicenseTypeId);

            entity.HasOne(d => d.DeploymentSide).WithMany(p => p.ConstructionLicenses).HasForeignKey(d => d.DeploymentSideId);

            entity.HasOne(d => d.DossierType).WithMany(p => p.ConstructionLicenses).HasForeignKey(d => d.DossierTypeId);

            entity.HasOne(d => d.Estate).WithMany(p => p.ConstructionLicenses).HasForeignKey(d => d.EstateId);

            entity.HasOne(d => d.ExcavationDangerStatus).WithMany(p => p.ConstructionLicenses).HasForeignKey(d => d.ExcavationDangerStatusId);

            entity.HasOne(d => d.FinalBuildingGroupSetting).WithMany(p => p.ConstructionLicenseFinalBuildingGroupSettings).HasForeignKey(d => d.FinalBuildingGroupSettingId);

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasForeignKey(d => d.ParentId);

            entity.HasOne(d => d.Status).WithMany(p => p.ConstructionLicenses).HasForeignKey(d => d.StatusId);

            entity.HasOne(d => d.Step).WithMany(p => p.ConstructionLicenses).HasForeignKey(d => d.StepId);

            entity.HasOne(d => d.StructureType).WithMany(p => p.ConstructionLicenses).HasForeignKey(d => d.StructureTypeId);

            entity.HasOne(d => d.UsageType).WithMany(p => p.ConstructionLicenses)
                .HasForeignKey(d => d.UsageTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ConstructionLicenseArea>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ConstructionLicenseArea");
        });

        modelBuilder.Entity<ConstructionLicenseComment>(entity =>
        {
            entity.ToTable("ConstructionLicenseComment", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_ConstructionLicenseComment_ConstructionLicenseId");

            entity.HasIndex(e => e.UserId, "IX_ConstructionLicenseComment_UserId");

            entity.Property(e => e.Description).HasMaxLength(1024);
            entity.Property(e => e.Title).HasMaxLength(512);

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.ConstructionLicenseComments).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.User).WithMany(p => p.ConstructionLicenseComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ConstructionLicenseDescription>(entity =>
        {
            entity.ToTable("ConstructionLicenseDescriptions", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_ConstructionLicenseDescriptions_ConstructionLicenseId");

            entity.HasIndex(e => e.DescriptionTypeId, "IX_ConstructionLicenseDescriptions_DescriptionTypeId");

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.ConstructionLicenseDescriptions).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.DescriptionType).WithMany(p => p.ConstructionLicenseDescriptions).HasForeignKey(d => d.DescriptionTypeId);
        });

        modelBuilder.Entity<ConstructionLicenseDocument>(entity =>
        {
            entity.ToTable("ConstructionLicenseDocuments", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_ConstructionLicenseDocuments_ConstructionLicenseId");

            entity.HasIndex(e => e.DocumentTypeId, "IX_ConstructionLicenseDocuments_DocumentTypeId");

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.ConstructionLicenseDocuments).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.DocumentType).WithMany(p => p.ConstructionLicenseDocuments).HasForeignKey(d => d.DocumentTypeId);
        });

        modelBuilder.Entity<ConstructionLicenseGeoLab>(entity =>
        {
            entity.ToTable("ConstructionLicenseGeoLabs", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_ConstructionLicenseGeoLabs_ConstructionLicenseId");

            entity.HasIndex(e => e.DrillingTypeId, "IX_ConstructionLicenseGeoLabs_DrillingTypeId");

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.ConstructionLicenseGeoLabs).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.DrillingType).WithMany(p => p.ConstructionLicenseGeoLabs).HasForeignKey(d => d.DrillingTypeId);
        });

        modelBuilder.Entity<ConstructionLicenseOption>(entity =>
        {
            entity.ToTable("ConstructionLicenseOptions", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_ConstructionLicenseOptions_ConstructionLicenseId");

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.ConstructionLicenseOptions).HasForeignKey(d => d.ConstructionLicenseId);
        });

        modelBuilder.Entity<ConstructionLicenseStatus>(entity =>
        {
            entity.ToTable("ConstructionLicenseStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ConstructionLicenseStep>(entity =>
        {
            entity.ToTable("ConstructionLicenseSteps", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ConstructionLicenseType>(entity =>
        {
            entity.ToTable("ConstructionLicenseTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ContactInfoType>(entity =>
        {
            entity.ToTable("ContactInfoTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.ToTable("Contracts", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_Contracts_ConstructionLicenseId");

            entity.HasIndex(e => e.ContractTypeId, "IX_Contracts_ContractTypeId");

            entity.HasIndex(e => e.StatusId, "IX_Contracts_StatusId");

            entity.Property(e => e.ContractNumber).HasMaxLength(128);
            entity.Property(e => e.SecretariatNumber).HasMaxLength(128);
            entity.Property(e => e.StatusId).HasDefaultValue(1);

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.Contracts).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.ContractType).WithMany(p => p.Contracts).HasForeignKey(d => d.ContractTypeId);

            entity.HasOne(d => d.Status).WithMany(p => p.Contracts).HasForeignKey(d => d.StatusId);
        });

        modelBuilder.Entity<ContractExpandation>(entity =>
        {
            entity.ToTable("ContractExpandation", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseDocumentId, "IX_ContractExpandation_ConstructionLicenseDocumentId");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_ContractExpandation_ConstructionLicenseId");

            entity.HasIndex(e => e.ContractExpandationStatusId, "IX_ContractExpandation_ContractExpandationStatusId");

            entity.HasIndex(e => e.ServiceFieldId, "IX_ContractExpandation_ServiceFieldId");

            entity.HasIndex(e => e.ServiceTypeId, "IX_ContractExpandation_ServiceTypeId");

            entity.Property(e => e.Description).HasMaxLength(2048);

            entity.HasOne(d => d.ConstructionLicenseDocument).WithMany(p => p.ContractExpandations).HasForeignKey(d => d.ConstructionLicenseDocumentId);

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.ContractExpandations).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.ContractExpandationStatus).WithMany(p => p.ContractExpandations).HasForeignKey(d => d.ContractExpandationStatusId);

            entity.HasOne(d => d.ServiceField).WithMany(p => p.ContractExpandations).HasForeignKey(d => d.ServiceFieldId);

            entity.HasOne(d => d.ServiceType).WithMany(p => p.ContractExpandations)
                .HasForeignKey(d => d.ServiceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ContractExpandationStatus>(entity =>
        {
            entity.ToTable("ContractExpandationStatus", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ContractPaymentDetail>(entity =>
        {
            entity.ToTable("ContractPaymentDetails", "cedo");

            entity.HasIndex(e => e.ContractId, "IX_ContractPaymentDetails_ContractId");

            entity.HasIndex(e => e.ContractPaymentForId, "IX_ContractPaymentDetails_ContractPaymentForId");

            entity.HasIndex(e => e.PaymentTypeId, "IX_ContractPaymentDetails_PaymentTypeId");

            entity.Property(e => e.BankName).HasMaxLength(50);
            entity.Property(e => e.ChequeNumber).HasMaxLength(20);
            entity.Property(e => e.Description).HasMaxLength(128);

            entity.HasOne(d => d.Contract).WithMany(p => p.ContractPaymentDetails).HasForeignKey(d => d.ContractId);

            entity.HasOne(d => d.ContractPaymentFor).WithMany(p => p.ContractPaymentDetails).HasForeignKey(d => d.ContractPaymentForId);

            entity.HasOne(d => d.PaymentType).WithMany(p => p.ContractPaymentDetails).HasForeignKey(d => d.PaymentTypeId);
        });

        modelBuilder.Entity<ContractPaymentFor>(entity =>
        {
            entity.ToTable("ContractPaymentFors", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ContractPaymentType>(entity =>
        {
            entity.ToTable("ContractPaymentTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ContractStatus>(entity =>
        {
            entity.ToTable("ContractStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ContractType>(entity =>
        {
            entity.ToTable("ContractTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<Controller>(entity =>
        {
            entity.HasIndex(e => e.ControllerTypeId, "IX_Controllers_ControllerTypeId");

            entity.HasIndex(e => e.EntityId, "IX_Controllers_EntityId");

            entity.HasIndex(e => e.ProjectId, "IX_Controllers_ProjectId");

            entity.Property(e => e.Area).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.ControllerType).WithMany(p => p.Controllers).HasForeignKey(d => d.ControllerTypeId);

            entity.HasOne(d => d.Entity).WithMany(p => p.Controllers).HasForeignKey(d => d.EntityId);

            entity.HasOne(d => d.Project).WithMany(p => p.Controllers).HasForeignKey(d => d.ProjectId);
        });

        modelBuilder.Entity<ControllerAction>(entity =>
        {
            entity.HasIndex(e => e.ActionTypeId, "IX_ControllerActions_ActionTypeId");

            entity.HasIndex(e => e.ControllerId, "IX_ControllerActions_ControllerId");

            entity.Property(e => e.ActionName).HasMaxLength(64);
            entity.Property(e => e.MethodName).HasMaxLength(64);

            entity.HasOne(d => d.ActionType).WithMany(p => p.ControllerActions).HasForeignKey(d => d.ActionTypeId);

            entity.HasOne(d => d.Controller).WithMany(p => p.ControllerActions).HasForeignKey(d => d.ControllerId);
        });

        modelBuilder.Entity<ControllerActionCode>(entity =>
        {
            entity.ToTable("ControllerActionCode");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.ControllerActionCode).HasForeignKey<ControllerActionCode>(d => d.Id);
        });

        modelBuilder.Entity<ControllerActionType>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(64);
        });

        modelBuilder.Entity<ControllerType>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(64);
        });

        modelBuilder.Entity<CostType>(entity =>
        {
            entity.ToTable("CostTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<CostTypeDetail>(entity =>
        {
            entity.ToTable("CostTypeDetails", "cedo");

            entity.HasIndex(e => e.CostTypeId, "IX_CostTypeDetails_CostTypeId");

            entity.HasIndex(e => e.ServiceFieldId, "IX_CostTypeDetails_ServiceFieldId");

            entity.HasOne(d => d.CostType).WithMany(p => p.CostTypeDetails).HasForeignKey(d => d.CostTypeId);

            entity.HasOne(d => d.ServiceField).WithMany(p => p.CostTypeDetails).HasForeignKey(d => d.ServiceFieldId);
        });

        modelBuilder.Entity<CostTypeDetailBill>(entity =>
        {
            entity.ToTable("CostTypeDetailBills", "cedo");

            entity.HasIndex(e => e.BillTypeId, "IX_CostTypeDetailBills_BillTypeId");

            entity.HasIndex(e => e.CostTypeDetailId, "IX_CostTypeDetailBills_CostTypeDetailId");

            entity.HasOne(d => d.BillType).WithMany(p => p.CostTypeDetailBills).HasForeignKey(d => d.BillTypeId);

            entity.HasOne(d => d.CostTypeDetail).WithMany(p => p.CostTypeDetailBills).HasForeignKey(d => d.CostTypeDetailId);
        });

        modelBuilder.Entity<CostTypeDetailSetting>(entity =>
        {
            entity.ToTable("CostTypeDetailSettings", "cedo");

            entity.HasIndex(e => e.BuildingSubGroupId, "IX_CostTypeDetailSettings_BuildingSubGroupId");

            entity.HasIndex(e => e.CostTypeDetailId, "IX_CostTypeDetailSettings_CostTypeDetailId");

            entity.HasOne(d => d.BuildingSubGroup).WithMany(p => p.CostTypeDetailSettings).HasForeignKey(d => d.BuildingSubGroupId);

            entity.HasOne(d => d.CostTypeDetail).WithMany(p => p.CostTypeDetailSettings).HasForeignKey(d => d.CostTypeDetailId);
        });

        modelBuilder.Entity<CostTypeOrder>(entity =>
        {
            entity.ToTable("CostTypeOrder", "cedo");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.CostTypeOrder).HasForeignKey<CostTypeOrder>(d => d.Id);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Courses", "cedo");

            entity.HasIndex(e => e.CourseTypeId, "IX_Courses_CourseTypeId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Title).HasMaxLength(128);

            entity.HasOne(d => d.CourseType).WithMany(p => p.Courses).HasForeignKey(d => d.CourseTypeId);
        });

        modelBuilder.Entity<CourseExecutor>(entity =>
        {
            entity.ToTable("CourseExecutors", "cedo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(128);
            entity.Property(e => e.ContractNumber).HasMaxLength(128);
            entity.Property(e => e.LicenceNumber).HasMaxLength(128);
            entity.Property(e => e.Tel).HasMaxLength(16);
            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Url).HasMaxLength(128);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.CourseExecutor).HasForeignKey<CourseExecutor>(d => d.Id);
        });

        modelBuilder.Entity<CourseHolding>(entity =>
        {
            entity.ToTable("CourseHoldings", "cedo");

            entity.HasIndex(e => e.CourseId, "IX_CourseHoldings_CourseId");

            entity.HasIndex(e => e.ExecutorId, "IX_CourseHoldings_ExecutorId");

            entity.HasIndex(e => e.StepId, "IX_CourseHoldings_StepId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ApprovalNumber).HasMaxLength(32);
            entity.Property(e => e.CertRegNumber).HasMaxLength(20);
            entity.Property(e => e.Description).HasMaxLength(512);
            entity.Property(e => e.Title).HasMaxLength(128);

            entity.HasOne(d => d.Course).WithMany(p => p.CourseHoldings)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Executor).WithMany(p => p.CourseHoldings).HasForeignKey(d => d.ExecutorId);

            entity.HasOne(d => d.Step).WithMany(p => p.CourseHoldings).HasForeignKey(d => d.StepId);
        });

        modelBuilder.Entity<CourseHoldingMember>(entity =>
        {
            entity.ToTable("CourseHoldingMembers", "cedo");

            entity.HasIndex(e => e.CourseHoldingId, "IX_CourseHoldingMembers_CourseHoldingId");

            entity.HasIndex(e => e.MemberId, "IX_CourseHoldingMembers_MemberId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.CourseHolding).WithMany(p => p.CourseHoldingMembers).HasForeignKey(d => d.CourseHoldingId);

            entity.HasOne(d => d.Member).WithMany(p => p.CourseHoldingMembers).HasForeignKey(d => d.MemberId);
        });

        modelBuilder.Entity<CourseHoldingSignatoriese>(entity =>
        {
            entity.ToTable("CourseHoldingSignatorieses", "cedo");

            entity.HasIndex(e => e.CourseHoldingId, "IX_CourseHoldingSignatorieses_CourseHoldingId");

            entity.HasIndex(e => e.SignatoryId, "IX_CourseHoldingSignatorieses_SignatoryId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.CourseHolding).WithMany(p => p.CourseHoldingSignatorieses).HasForeignKey(d => d.CourseHoldingId);

            entity.HasOne(d => d.Signatory).WithMany(p => p.CourseHoldingSignatorieses).HasForeignKey(d => d.SignatoryId);
        });

        modelBuilder.Entity<CourseHoldingStep>(entity =>
        {
            entity.ToTable("CourseHoldingSteps", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<CourseLesson>(entity =>
        {
            entity.ToTable("CourseLessons", "cedo");

            entity.HasIndex(e => e.CourseId, "IX_CourseLessons_CourseId");

            entity.HasIndex(e => e.LessonId, "IX_CourseLessons_LessonId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Course).WithMany(p => p.CourseLessons).HasForeignKey(d => d.CourseId);

            entity.HasOne(d => d.Lesson).WithMany(p => p.CourseLessons).HasForeignKey(d => d.LessonId);
        });

        modelBuilder.Entity<CourseQualifiication>(entity =>
        {
            entity.ToTable("CourseQualifiications", "cedo");

            entity.HasIndex(e => e.CourseId, "IX_CourseQualifiications_CourseId");

            entity.HasIndex(e => e.EngGradeId, "IX_CourseQualifiications_EngGradeId");

            entity.HasIndex(e => e.ServiceFieldId, "IX_CourseQualifiications_ServiceFieldId");

            entity.HasIndex(e => e.ServiceTypeId, "IX_CourseQualifiications_ServiceTypeId");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseQualifiications).HasForeignKey(d => d.CourseId);

            entity.HasOne(d => d.EngGrade).WithMany(p => p.CourseQualifiications).HasForeignKey(d => d.EngGradeId);

            entity.HasOne(d => d.ServiceField).WithMany(p => p.CourseQualifiications).HasForeignKey(d => d.ServiceFieldId);

            entity.HasOne(d => d.ServiceType).WithMany(p => p.CourseQualifiications).HasForeignKey(d => d.ServiceTypeId);
        });

        modelBuilder.Entity<CourseType>(entity =>
        {
            entity.ToTable("CourseTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<DashboardGadget>(entity =>
        {
            entity.HasIndex(e => e.GadgetViewId, "IX_DashboardGadgets_GadgetViewId");

            entity.HasIndex(e => e.UserId, "IX_DashboardGadgets_UserId");

            entity.HasOne(d => d.GadgetView).WithMany(p => p.DashboardGadgets)
                .HasForeignKey(d => d.GadgetViewId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.User).WithMany(p => p.DashboardGadgets).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<DataSourceDiagram>(entity =>
        {
            entity.HasIndex(e => e.DataSourceId, "IX_DataSourceDiagrams_DataSourceId");

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.DataSource).WithMany(p => p.DataSourceDiagrams)
                .HasForeignKey(d => d.DataSourceId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DataSourceEntity>(entity =>
        {
            entity.HasIndex(e => e.ProjectDataSourceId, "IX_DataSourceEntities_ProjectDataSourceId");

            entity.Property(e => e.EntityName)
                .HasMaxLength(128)
                .HasDefaultValue("");
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.TableName).HasMaxLength(128);

            entity.HasOne(d => d.ProjectDataSource).WithMany(p => p.DataSourceEntities).HasForeignKey(d => d.ProjectDataSourceId);
        });

        modelBuilder.Entity<DataSourceType>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<DecisionTable>(entity =>
        {
            entity.HasIndex(e => e.ArchivedById, "IX_DecisionTables_ArchivedById");

            entity.HasIndex(e => e.Name, "IX_DecisionTables_Name");

            entity.HasIndex(e => e.ProjectId, "IX_DecisionTables_ProjectId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(256);

            entity.HasOne(d => d.ArchivedBy).WithMany(p => p.DecisionTables).HasForeignKey(d => d.ArchivedById);

            entity.HasOne(d => d.Project).WithMany(p => p.DecisionTables).HasForeignKey(d => d.ProjectId);
        });

        modelBuilder.Entity<DecisionTableTask>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.TableName).HasMaxLength(256);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.DecisionTableTask).HasForeignKey<DecisionTableTask>(d => d.Id);
        });

        modelBuilder.Entity<DeletedAttendee>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("deleted_attendees");

            entity.Property(e => e.FirstName).HasMaxLength(128);
            entity.Property(e => e.LastName).HasMaxLength(128);
            entity.Property(e => e.MembershipCode).HasMaxLength(32);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments", "puyahlpdsk");

            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<DepartmentUser>(entity =>
        {
            entity.ToTable("DepartmentUsers", "puyahlpdsk");

            entity.HasIndex(e => e.DepartmentId, "IX_DepartmentUsers_DepartmentId");

            entity.HasIndex(e => e.UserAccountId, "IX_DepartmentUsers_UserAccountId");

            entity.HasOne(d => d.Department).WithMany(p => p.DepartmentUsers).HasForeignKey(d => d.DepartmentId);

            entity.HasOne(d => d.UserAccount).WithMany(p => p.DepartmentUsers).HasForeignKey(d => d.UserAccountId);
        });

        modelBuilder.Entity<DeploymentSide>(entity =>
        {
            entity.ToTable("DeploymentSides", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<DescriptionType>(entity =>
        {
            entity.ToTable("DescriptionTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<DiagramEntity>(entity =>
        {
            entity.HasIndex(e => e.DiagramId, "IX_DiagramEntities_DiagramId");

            entity.HasIndex(e => e.EntityId, "IX_DiagramEntities_EntityId");

            entity.HasOne(d => d.Diagram).WithMany(p => p.DiagramEntities).HasForeignKey(d => d.DiagramId);

            entity.HasOne(d => d.Entity).WithMany(p => p.DiagramEntities).HasForeignKey(d => d.EntityId);
        });

        modelBuilder.Entity<DocumentFinder>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("DocumentFinder");

            entity.Property(e => e.DossierNumber).HasMaxLength(64);
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<DocumentIndex>(entity =>
        {
            entity.HasIndex(e => e.HelpId, "IX_DocumentIndices_HelpId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Title).HasMaxLength(1024);

            entity.HasOne(d => d.Help).WithMany(p => p.DocumentIndices).HasForeignKey(d => d.HelpId);

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasForeignKey(d => d.ParentId);
        });

        modelBuilder.Entity<DocumentType>(entity =>
        {
            entity.ToTable("DocumentTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<DossierBuildingGroupSetting>(entity =>
        {
            entity.ToTable("DossierBuildingGroupSettings", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_DossierBuildingGroupSettings_ConstructionLicenseId");

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.DossierBuildingGroupSettings).HasForeignKey(d => d.ConstructionLicenseId);
        });

        modelBuilder.Entity<DossierCostSetting>(entity =>
        {
            entity.ToTable("DossierCostSettings", "cedo");

            entity.HasIndex(e => e.BillTypeId, "IX_DossierCostSettings_BillTypeId");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_DossierCostSettings_ConstructionLicenseId");

            entity.Property(e => e.BuildingGroup).HasMaxLength(128);
            entity.Property(e => e.FloorCount).HasMaxLength(128);
            entity.Property(e => e.Metraje).HasMaxLength(128);

            entity.HasOne(d => d.BillType).WithMany(p => p.DossierCostSettings).HasForeignKey(d => d.BillTypeId);

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.DossierCostSettings).HasForeignKey(d => d.ConstructionLicenseId);
        });

        modelBuilder.Entity<DossierCostSettingDetail>(entity =>
        {
            entity.ToTable("DossierCostSettingDetails", "cedo");

            entity.HasIndex(e => e.DossierCostSettingId, "IX_DossierCostSettingDetails_DossierCostSettingId");

            entity.HasIndex(e => e.TariffDetailItemId, "IX_DossierCostSettingDetails_TariffDetailItemId");

            entity.HasOne(d => d.DossierCostSetting).WithMany(p => p.DossierCostSettingDetails).HasForeignKey(d => d.DossierCostSettingId);

            entity.HasOne(d => d.TariffDetailItem).WithMany(p => p.DossierCostSettingDetails).HasForeignKey(d => d.TariffDetailItemId);
        });

        modelBuilder.Entity<DossierType>(entity =>
        {
            entity.ToTable("DossierType", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<DrillingType>(entity =>
        {
            entity.ToTable("DrillingTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<DuplicatedMember>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Duplicated-Members");

            entity.Property(e => e.Count).HasColumnName("count");
        });

        modelBuilder.Entity<ElectricalExecutiveOperationsForm>(entity =>
        {
            entity.ToTable("ElectricalExecutiveOperationsForms", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_ElectricalExecutiveOperationsForms_InvolvedMemberId");

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.ElectricalExecutiveOperationsForms)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ElectricalPlanControlForm>(entity =>
        {
            entity.ToTable("ElectricalPlanControlForms", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_ElectricalPlanControlForms_InvolvedMemberId");

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.ElectricalPlanControlForms)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ElectricalRedLinesForm>(entity =>
        {
            entity.ToTable("ElectricalRedLinesForms", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_ElectricalRedLinesForms_InvolvedMemberId");

            entity.Property(e => e.Description).HasMaxLength(1024);

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.ElectricalRedLinesForms)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ElectricalSpecForm>(entity =>
        {
            entity.ToTable("ElectricalSpecForms", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_ElectricalSpecForms_InvolvedMemberId");

            entity.Property(e => e.Cctv).HasColumnName("CCTV");
            entity.Property(e => e.Tvantenna).HasColumnName("TVAntenna");
            entity.Property(e => e.TvantennaStandard).HasColumnName("TVAntennaStandard");

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.ElectricalSpecForms)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ElectricitySafetyForm>(entity =>
        {
            entity.ToTable("ElectricitySafetyForms", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_ElectricitySafetyForms_ConstructionLicenseId");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_ElectricitySafetyForms_InvolvedMemberId");

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.ElectricitySafetyForms).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.ElectricitySafetyForms).HasForeignKey(d => d.InvolvedMemberId);
        });

        modelBuilder.Entity<Elevator>(entity =>
        {
            entity.ToTable("Elevators", "cedo");

            entity.HasIndex(e => e.ElectricalSpecFormId, "IX_Elevators_ElectricalSpecFormId");

            entity.Property(e => e.ExecutionQuality).HasDefaultValue(true);
            entity.Property(e => e.FastRpm).HasColumnName("FastRPM");
            entity.Property(e => e.LowRpm).HasColumnName("LowRPM");
            entity.Property(e => e.StandardElevatorEquipment).HasDefaultValue(true);

            entity.HasOne(d => d.ElectricalSpecForm).WithMany(p => p.Elevators).HasForeignKey(d => d.ElectricalSpecFormId);
        });

        modelBuilder.Entity<EmailAccount>(entity =>
        {
            entity.Property(e => e.From).HasMaxLength(512);
            entity.Property(e => e.Password).HasMaxLength(128);
            entity.Property(e => e.SmtpServer).HasMaxLength(512);
            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.UserName).HasMaxLength(128);
        });

        modelBuilder.Entity<EmploymentHistory>(entity =>
        {
            entity.ToTable("EmploymentHistories", "cedo");

            entity.HasIndex(e => e.MemberId, "IX_EmploymentHistories_MemberId");

            entity.Property(e => e.Description).HasMaxLength(512);

            entity.HasOne(d => d.Member).WithMany(p => p.EmploymentHistories).HasForeignKey(d => d.MemberId);
        });

        modelBuilder.Entity<EngServiceCostScaleSetting>(entity =>
        {
            entity.ToTable("EngServiceCostScaleSettings", "cedo");

            entity.HasIndex(e => e.DossierTypeId, "IX_EngServiceCostScaleSettings_DossierTypeId");

            entity.HasOne(d => d.DossierType).WithMany(p => p.EngServiceCostScaleSettings)
                .HasForeignKey(d => d.DossierTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<EngServiceTariff>(entity =>
        {
            entity.ToTable("EngServiceTariffs", "cedo");

            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<EngServiceTariffDetail>(entity =>
        {
            entity.ToTable("EngServiceTariffDetails", "cedo");

            entity.HasIndex(e => e.BuildingSubGroupId, "IX_EngServiceTariffDetails_BuildingSubGroupId");

            entity.HasIndex(e => e.EngServiceTariffId, "IX_EngServiceTariffDetails_EngServiceTariffId");

            entity.HasOne(d => d.BuildingSubGroup).WithMany(p => p.EngServiceTariffDetails)
                .HasForeignKey(d => d.BuildingSubGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.EngServiceTariff).WithMany(p => p.EngServiceTariffDetails).HasForeignKey(d => d.EngServiceTariffId);
        });

        modelBuilder.Entity<EngServiceTariffDetailItem>(entity =>
        {
            entity.ToTable("EngServiceTariffDetailItems", "cedo");

            entity.HasIndex(e => e.BillTypeId, "IX_EngServiceTariffDetailItems_BillTypeId");

            entity.HasIndex(e => e.CostTypeDetailId, "IX_EngServiceTariffDetailItems_CostTypeDetailId");

            entity.HasIndex(e => e.EngServiceTariffDetailId, "IX_EngServiceTariffDetailItems_EngServiceTariffDetailId");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.BillType).WithMany(p => p.EngServiceTariffDetailItems)
                .HasForeignKey(d => d.BillTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.CostTypeDetail).WithMany(p => p.EngServiceTariffDetailItems).HasForeignKey(d => d.CostTypeDetailId);

            entity.HasOne(d => d.EngServiceTariffDetail).WithMany(p => p.EngServiceTariffDetailItems)
                .HasForeignKey(d => d.EngServiceTariffDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<EngineeringGrade>(entity =>
        {
            entity.ToTable("EngineeringGrades", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<EntityProperty>(entity =>
        {
            entity.HasIndex(e => e.ProjectEntityId, "IX_EntityProperties_ProjectEntityId");

            entity.HasIndex(e => e.Type, "IX_EntityProperties_Type");

            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasDefaultValue("");
            entity.Property(e => e.Type).HasMaxLength(128);

            entity.HasOne(d => d.ProjectEntity).WithMany(p => p.EntityProperties).HasForeignKey(d => d.ProjectEntityId);
        });

        modelBuilder.Entity<EnumEntityItem>(entity =>
        {
            entity.HasIndex(e => e.EntityId, "IX_EnumEntityItems_EntityId");

            entity.Property(e => e.ItemDisplay).HasMaxLength(128);
            entity.Property(e => e.ItemValue).HasMaxLength(128);

            entity.HasOne(d => d.Entity).WithMany(p => p.EnumEntityItems).HasForeignKey(d => d.EntityId);
        });

        modelBuilder.Entity<ErrorCatchEvent>(entity =>
        {
            entity.ToTable("ErrorCatchEvent");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ErrorType).HasMaxLength(1024);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.ErrorCatchEvent).HasForeignKey<ErrorCatchEvent>(d => d.Id);
        });

        modelBuilder.Entity<Estate>(entity =>
        {
            entity.ToTable("Estates", "cedo");

            entity.HasIndex(e => e.Code, "IX_Estates_Code_CFF1E");

            entity.HasIndex(e => e.MunicipalityId, "IX_Estates_MunicipalityId");

            entity.HasIndex(e => new { e.MunicipalityId, e.Code }, "IX_Estates_MunicipalityId_Code_06151");

            entity.HasIndex(e => e.OwnershipDocumentTypeId, "IX_Estates_OwnershipDocumentTypeId");

            entity.Property(e => e.Alley).HasMaxLength(128);
            entity.Property(e => e.Code).HasMaxLength(128);
            entity.Property(e => e.District).HasMaxLength(128);
            entity.Property(e => e.MainPlaque)
                .HasMaxLength(100)
                .HasDefaultValue("-");
            entity.Property(e => e.OwnershipDocumentTypeId).HasDefaultValue(1);
            entity.Property(e => e.PartPlaque)
                .HasMaxLength(64)
                .HasDefaultValue("-");
            entity.Property(e => e.PiecePlaque).HasMaxLength(100);
            entity.Property(e => e.Plaque).HasMaxLength(20);
            entity.Property(e => e.PostalCode).HasMaxLength(15);
            entity.Property(e => e.SecondaryPlaque)
                .HasMaxLength(100)
                .HasDefaultValue("-");
            entity.Property(e => e.Street)
                .HasMaxLength(128)
                .HasDefaultValue("-");

            entity.HasOne(d => d.Municipality).WithMany(p => p.Estates).HasForeignKey(d => d.MunicipalityId);

            entity.HasOne(d => d.OwnershipDocumentType).WithMany(p => p.Estates).HasForeignKey(d => d.OwnershipDocumentTypeId);
        });

        modelBuilder.Entity<EstateCoordinate>(entity =>
        {
            entity.ToTable("EstateCoordinates", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_EstateCoordinates_ConstructionLicenseId");

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.EstateCoordinates).HasForeignKey(d => d.ConstructionLicenseId);
        });

        modelBuilder.Entity<Examination>(entity =>
        {
            entity.ToTable("Examinations", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_Examinations_ConstructionLicenseId");

            entity.HasIndex(e => e.ExaminationFileId, "IX_Examinations_ExaminationFileId");

            entity.HasIndex(e => e.ExaminationPlaceId, "IX_Examinations_ExaminationPlaceId");

            entity.HasIndex(e => e.ExaminationStatusId, "IX_Examinations_ExaminationStatusId");

            entity.HasIndex(e => e.ExaminationTypeId, "IX_Examinations_ExaminationTypeId");

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.Examinations).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.ExaminationFile).WithMany(p => p.Examinations).HasForeignKey(d => d.ExaminationFileId);

            entity.HasOne(d => d.ExaminationPlace).WithMany(p => p.Examinations).HasForeignKey(d => d.ExaminationPlaceId);

            entity.HasOne(d => d.ExaminationStatus).WithMany(p => p.Examinations).HasForeignKey(d => d.ExaminationStatusId);

            entity.HasOne(d => d.ExaminationType).WithMany(p => p.Examinations).HasForeignKey(d => d.ExaminationTypeId);
        });

        modelBuilder.Entity<ExaminationPlace>(entity =>
        {
            entity.ToTable("ExaminationPlaces", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ExaminationStatus>(entity =>
        {
            entity.ToTable("ExaminationStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ExaminationType>(entity =>
        {
            entity.ToTable("ExaminationTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ExcavationDangerStatus>(entity =>
        {
            entity.ToTable("ExcavationDangerStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ExecuterCompetenceSetting>(entity =>
        {
            entity.ToTable("ExecuterCompetenceSettings", "cedo");

            entity.HasIndex(e => e.GradeId, "IX_ExecuterCompetenceSettings_GradeId");

            entity.HasIndex(e => e.MemberShipTypeId, "IX_ExecuterCompetenceSettings_MemberShipTypeId");

            entity.HasIndex(e => e.MemberTypeId, "IX_ExecuterCompetenceSettings_MemberTypeId");

            entity.HasIndex(e => e.ServiceFieldId, "IX_ExecuterCompetenceSettings_ServiceFieldId");

            entity.HasIndex(e => e.ServiceTypeId, "IX_ExecuterCompetenceSettings_ServiceTypeId");

            entity.Property(e => e.MemberShipTypeId).HasDefaultValue(1);
            entity.Property(e => e.ServiceTypeId).HasDefaultValue(7);

            entity.HasOne(d => d.Grade).WithMany(p => p.ExecuterCompetenceSettings).HasForeignKey(d => d.GradeId);

            entity.HasOne(d => d.MemberShipType).WithMany(p => p.ExecuterCompetenceSettings)
                .HasForeignKey(d => d.MemberShipTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.MemberType).WithMany(p => p.ExecuterCompetenceSettings).HasForeignKey(d => d.MemberTypeId);

            entity.HasOne(d => d.ServiceField).WithMany(p => p.ExecuterCompetenceSettings).HasForeignKey(d => d.ServiceFieldId);

            entity.HasOne(d => d.ServiceType).WithMany(p => p.ExecuterCompetenceSettings)
                .HasForeignKey(d => d.ServiceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ExecuterContract>(entity =>
        {
            entity.ToTable("ExecuterContracts", "cedo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Item21).HasMaxLength(1024);
            entity.Property(e => e.ManagerFund).HasDefaultValue(0L);
            entity.Property(e => e.ManagerFundGurantee).HasDefaultValue(0L);
            entity.Property(e => e.ManagerWageGuarantee).HasDefaultValue(0.0);
            entity.Property(e => e.ManagerWagePercent).HasDefaultValue(0.0);
            entity.Property(e => e.PurposeDescription).HasMaxLength(1024);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.ExecuterContract).HasForeignKey<ExecuterContract>(d => d.Id);
        });

        modelBuilder.Entity<ExecuterContractSuspensionRequest>(entity =>
        {
            entity.ToTable("ExecuterContractSuspensionRequest", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_ExecuterContractSuspensionRequest_ConstructionLicenseId");

            entity.HasIndex(e => e.StatusId, "IX_ExecuterContractSuspensionRequest_StatusId");

            entity.Property(e => e.ExecuterDescription).HasMaxLength(2048);
            entity.Property(e => e.OwnerDescription).HasMaxLength(2048);

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.ExecuterContractSuspensionRequests).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.Status).WithMany(p => p.ExecuterContractSuspensionRequests).HasForeignKey(d => d.StatusId);
        });

        modelBuilder.Entity<ExecuterContractSuspensionRequestStatus>(entity =>
        {
            entity.ToTable("ExecuterContractSuspensionRequestStatus", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ExecuterContractTermination>(entity =>
        {
            entity.ToTable("ExecuterContractTermination", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_ExecuterContractTermination_ConstructionLicenseId");

            entity.HasIndex(e => e.StatusId, "IX_ExecuterContractTermination_StatusId");

            entity.Property(e => e.ArbitrationDescription).HasMaxLength(1024);
            entity.Property(e => e.Description).HasMaxLength(1024);

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.ExecuterContractTerminations).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.Status).WithMany(p => p.ExecuterContractTerminations).HasForeignKey(d => d.StatusId);
        });

        modelBuilder.Entity<ExecuterContractTerminationJudgment>(entity =>
        {
            entity.ToTable("ExecuterContractTerminationJudgment", "cedo");

            entity.HasIndex(e => e.ExecuterContractTerminationId, "IX_ExecuterContractTerminationJudgment_ExecuterContractTerminationId");

            entity.HasIndex(e => e.MemberServiceId, "IX_ExecuterContractTerminationJudgment_MemberServiceId");

            entity.HasIndex(e => e.ResultFileId, "IX_ExecuterContractTerminationJudgment_ResultFileId");

            entity.Property(e => e.Result).HasMaxLength(2048);

            entity.HasOne(d => d.ExecuterContractTermination).WithMany(p => p.ExecuterContractTerminationJudgments).HasForeignKey(d => d.ExecuterContractTerminationId);

            entity.HasOne(d => d.MemberService).WithMany(p => p.ExecuterContractTerminationJudgments)
                .HasForeignKey(d => d.MemberServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ResultFile).WithMany(p => p.ExecuterContractTerminationJudgments).HasForeignKey(d => d.ResultFileId);
        });

        modelBuilder.Entity<ExecuterContractTerminationStatus>(entity =>
        {
            entity.ToTable("ExecuterContractTerminationStatus", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ExpertCostsSetting>(entity =>
        {
            entity.ToTable("ExpertCostsSetting", "cedo");

            entity.HasIndex(e => e.CityId, "IX_ExpertCostsSetting_CityId");

            entity.HasOne(d => d.City).WithMany(p => p.ExpertCostsSettings).HasForeignKey(d => d.CityId);
        });

        modelBuilder.Entity<ExternalReport>(entity =>
        {
            entity.ToTable("ExternalReports", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_ExternalReports_ConstructionLicenseId");

            entity.HasIndex(e => e.ExternalReportStatusId, "IX_ExternalReports_ExternalReportStatusId");

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.ExternalReports).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.ExternalReportStatus).WithMany(p => p.ExternalReports).HasForeignKey(d => d.ExternalReportStatusId);
        });

        modelBuilder.Entity<ExternalReportStatus>(entity =>
        {
            entity.ToTable("ExternalReportStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<FacilityDeviceType>(entity =>
        {
            entity.ToTable("FacilityDeviceTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<FacilityDevicesSpecification>(entity =>
        {
            entity.ToTable("FacilityDevicesSpecifications", "cedo");

            entity.HasIndex(e => e.DeviceTypeId, "IX_FacilityDevicesSpecifications_DeviceTypeId");

            entity.HasIndex(e => e.FormId, "IX_FacilityDevicesSpecifications_FormId");

            entity.Property(e => e.Description).HasMaxLength(1024);

            entity.HasOne(d => d.DeviceType).WithMany(p => p.FacilityDevicesSpecifications).HasForeignKey(d => d.DeviceTypeId);

            entity.HasOne(d => d.Form).WithMany(p => p.FacilityDevicesSpecifications).HasForeignKey(d => d.FormId);
        });

        modelBuilder.Entity<FacilityRequest>(entity =>
        {
            entity.ToTable("FacilityRequests", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_FacilityRequests_ConstructionLicenseId");

            entity.HasIndex(e => e.RequestForId, "IX_FacilityRequests_RequestForId");

            entity.HasIndex(e => e.RequesterId, "IX_FacilityRequests_RequesterId");

            entity.HasIndex(e => e.StatusId, "IX_FacilityRequests_StatusId");

            entity.Property(e => e.Description).HasMaxLength(1000);

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.FacilityRequests).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.RequestFor).WithMany(p => p.FacilityRequests).HasForeignKey(d => d.RequestForId);

            entity.HasOne(d => d.Requester).WithMany(p => p.FacilityRequests).HasForeignKey(d => d.RequesterId);

            entity.HasOne(d => d.Status).WithMany(p => p.FacilityRequests).HasForeignKey(d => d.StatusId);
        });

        modelBuilder.Entity<FacilityRequestFor>(entity =>
        {
            entity.ToTable("FacilityRequestFors", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<FacilityRequestStatus>(entity =>
        {
            entity.ToTable("FacilityRequestStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<FirewallRule>(entity =>
        {
            entity.HasIndex(e => e.FirewallId, "IX_FirewallRules_FirewallId");

            entity.Property(e => e.DenyMessage).HasMaxLength(2048);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.Firewall).WithMany(p => p.FirewallRules).HasForeignKey(d => d.FirewallId);
        });

        modelBuilder.Entity<FiscalBalance>(entity =>
        {
            entity.ToTable("FiscalBalances", "cedo");

            entity.HasIndex(e => e.MemberId, "IX_FiscalBalances_MemberId");

            entity.HasIndex(e => e.ServiceFieldId, "IX_FiscalBalances_ServiceFieldId");

            entity.HasIndex(e => e.ServiceTypeId, "IX_FiscalBalances_ServiceTypeId");

            entity.HasOne(d => d.Member).WithMany(p => p.FiscalBalances).HasForeignKey(d => d.MemberId);

            entity.HasOne(d => d.ServiceField).WithMany(p => p.FiscalBalances).HasForeignKey(d => d.ServiceFieldId);

            entity.HasOne(d => d.ServiceType).WithMany(p => p.FiscalBalances).HasForeignKey(d => d.ServiceTypeId);
        });

        modelBuilder.Entity<Floor>(entity =>
        {
            entity.ToTable("Floors", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_Floors_ConstructionLicenseId");

            entity.HasIndex(e => e.FloorRegisterStepId, "IX_Floors_FloorRegisterStepId");

            entity.HasIndex(e => e.FloorTypeId, "IX_Floors_FloorTypeId");

            entity.Property(e => e.FloorRegisterStepId).HasDefaultValue(1);

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.Floors).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.FloorRegisterStep).WithMany(p => p.Floors).HasForeignKey(d => d.FloorRegisterStepId);

            entity.HasOne(d => d.FloorType).WithMany(p => p.Floors).HasForeignKey(d => d.FloorTypeId);
        });

        modelBuilder.Entity<Floor1>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Floors");

            entity.Property(e => e.DossierNumber).HasMaxLength(64);
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<FloorRegisterStep>(entity =>
        {
            entity.ToTable("FloorRegisterStep", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<FloorType>(entity =>
        {
            entity.ToTable("FloorTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<Flow>(entity =>
        {
            entity.HasIndex(e => e.ProjectId, "IX_Flows_ProjectId");

            entity.Property(e => e.Group).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.RouteParam).HasMaxLength(128);

            entity.HasOne(d => d.Project).WithMany(p => p.Flows).HasForeignKey(d => d.ProjectId);
        });

        modelBuilder.Entity<FlowActor>(entity =>
        {
            entity.HasIndex(e => e.FlowVersionId, "IX_FlowActors_FlowVersionId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Color).HasMaxLength(9);
            entity.Property(e => e.Title).HasMaxLength(128);

            entity.HasOne(d => d.FlowVersion).WithMany(p => p.FlowActors)
                .HasForeignKey(d => d.FlowVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FlowConnectionType>(entity =>
        {
            entity.ToTable("FlowConnectionType");

            entity.Property(e => e.Title).HasMaxLength(32);
        });

        modelBuilder.Entity<FlowConnector>(entity =>
        {
            entity.HasIndex(e => e.ConnectionTypeId, "IX_FlowConnectors_ConnectionTypeId");

            entity.HasIndex(e => e.SourceNodeId, "IX_FlowConnectors_SourceNodeId");

            entity.HasIndex(e => e.TargetNodeId, "IX_FlowConnectors_TargetNodeId");

            entity.HasIndex(e => e.UniqueId, "idx_UniqueId");

            entity.Property(e => e.DiagramLayoutSourcePort).HasMaxLength(128);
            entity.Property(e => e.DiagramLayoutTargetPort).HasMaxLength(128);
            entity.Property(e => e.Title).HasMaxLength(128);

            entity.HasOne(d => d.ConnectionType).WithMany(p => p.FlowConnectors).HasForeignKey(d => d.ConnectionTypeId);

            entity.HasOne(d => d.SourceNode).WithMany(p => p.FlowConnectorSourceNodes).HasForeignKey(d => d.SourceNodeId);

            entity.HasOne(d => d.TargetNode).WithMany(p => p.FlowConnectorTargetNodes)
                .HasForeignKey(d => d.TargetNodeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FlowEvent>(entity =>
        {
            entity.HasIndex(e => e.EventModeId, "IX_FlowEvents_EventModeId");

            entity.HasIndex(e => e.EventTriggerId, "IX_FlowEvents_EventTriggerId");

            entity.HasIndex(e => e.EventTypeId, "IX_FlowEvents_EventTypeId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.EventTriggerId).HasDefaultValue(1);

            entity.HasOne(d => d.EventMode).WithMany(p => p.FlowEvents).HasForeignKey(d => d.EventModeId);

            entity.HasOne(d => d.EventTrigger).WithMany(p => p.FlowEvents).HasForeignKey(d => d.EventTriggerId);

            entity.HasOne(d => d.EventType).WithMany(p => p.FlowEvents).HasForeignKey(d => d.EventTypeId);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.FlowEvent).HasForeignKey<FlowEvent>(d => d.Id);
        });

        modelBuilder.Entity<FlowEventMode>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(32);
        });

        modelBuilder.Entity<FlowEventTrigger>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(32);
        });

        modelBuilder.Entity<FlowEventType>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(32);
        });

        modelBuilder.Entity<FlowGatewaie>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.FlowGatewaie).HasForeignKey<FlowGatewaie>(d => d.Id);
        });

        modelBuilder.Entity<FlowInstance>(entity =>
        {
            entity.HasIndex(e => e.FlowVersionId, "IX_FlowInstances_FlowVersionId");

            entity.HasIndex(e => e.FlowVersionId, "IX_FlowInstances_FlowVersionId_EA89C");

            entity.HasIndex(e => new { e.FlowVersionId, e.StatusId }, "IX_FlowInstances_FlowVersionId_StatusId_0F0A3");

            entity.HasIndex(e => e.ParentInstanceId, "IX_FlowInstances_ParentInstanceId");

            entity.HasIndex(e => e.StatusId, "IX_FlowInstances_StatusId");

            entity.HasIndex(e => e.StatusId, "IX_FlowInstances_StatusId_2D57B");

            entity.HasIndex(e => e.StatusId, "IX_FlowInstances_StatusId_A387A");

            entity.HasIndex(e => e.UserGroupId, "IX_FlowInstances_UserGroupId");

            entity.HasIndex(e => new { e.ParentInstanceId, e.StatusId }, "MissingIndex_1");

            entity.HasIndex(e => e.ParentInstanceId, "NonClusteredIndex-20211119-181756");

            entity.Property(e => e.Title).HasMaxLength(512);

            entity.HasOne(d => d.FlowVersion).WithMany(p => p.FlowInstances).HasForeignKey(d => d.FlowVersionId);

            entity.HasOne(d => d.ParentInstance).WithMany(p => p.InverseParentInstance).HasForeignKey(d => d.ParentInstanceId);

            entity.HasOne(d => d.Status).WithMany(p => p.FlowInstances).HasForeignKey(d => d.StatusId);

            entity.HasOne(d => d.UserGroup).WithMany(p => p.FlowInstances).HasForeignKey(d => d.UserGroupId);
        });

        modelBuilder.Entity<FlowInstanceLog>(entity =>
        {
            entity.HasIndex(e => e.FlowInstanceId, "IX_FlowInstanceLogs_FlowInstanceId");

            entity.HasIndex(e => e.LogTypeId, "IX_FlowInstanceLogs_LogTypeId");

            entity.HasOne(d => d.FlowInstance).WithMany(p => p.FlowInstanceLogs).HasForeignKey(d => d.FlowInstanceId);

            entity.HasOne(d => d.LogType).WithMany(p => p.FlowInstanceLogs).HasForeignKey(d => d.LogTypeId);
        });

        modelBuilder.Entity<FlowInstancePauseLog>(entity =>
        {
            entity.HasIndex(e => e.InstanceId, "IX_FlowInstancePauseLogs_InstanceId");

            entity.Property(e => e.Description).HasMaxLength(1000);

            entity.HasOne(d => d.Instance).WithMany(p => p.FlowInstancePauseLogs).HasForeignKey(d => d.InstanceId);
        });

        modelBuilder.Entity<FlowInstanceStatus>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<FlowInstanceVariable>(entity =>
        {
            entity.HasIndex(e => new { e.FlowInstanceId, e.Name }, "IX_FlowInstanceVariables_FlowInstanceId_Name");

            entity.HasIndex(e => e.Name, "IX_FlowInstanceVariables_Name");

            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.FlowInstance).WithMany(p => p.FlowInstanceVariables).HasForeignKey(d => d.FlowInstanceId);
        });

        modelBuilder.Entity<FlowNode>(entity =>
        {
            entity.HasIndex(e => e.ActorId, "IX_FlowNodes_ActorId");

            entity.HasIndex(e => e.Code, "IX_FlowNodes_Code_4DDE4");

            entity.HasIndex(e => e.FlowVersionId, "IX_FlowNodes_FlowVersionId");

            entity.HasIndex(e => e.FlowVersionId, "IX_FlowNodes_FlowVersionId_B2253");

            entity.HasIndex(e => e.NodeTypeId, "IX_FlowNodes_NodeTypeId");

            entity.HasIndex(e => e.UniqueId, "idx_UniqueId");

            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.Actor).WithMany(p => p.FlowNodes)
                .HasForeignKey(d => d.ActorId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.FlowVersion).WithMany(p => p.FlowNodes).HasForeignKey(d => d.FlowVersionId);

            entity.HasOne(d => d.NodeType).WithMany(p => p.FlowNodes).HasForeignKey(d => d.NodeTypeId);
        });

        modelBuilder.Entity<FlowNodeLifeCycleEvent>(entity =>
        {
            entity.HasIndex(e => e.EventTypeId, "IX_FlowNodeLifeCycleEvents_EventTypeId");

            entity.HasIndex(e => e.NodeId, "IX_FlowNodeLifeCycleEvents_NodeId");

            entity.HasOne(d => d.EventType).WithMany(p => p.FlowNodeLifeCycleEvents).HasForeignKey(d => d.EventTypeId);

            entity.HasOne(d => d.Node).WithMany(p => p.FlowNodeLifeCycleEvents).HasForeignKey(d => d.NodeId);
        });

        modelBuilder.Entity<FlowNodeLifeCycleEventType>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<FlowNodeType>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(32);
        });

        modelBuilder.Entity<FlowStarterTask>(entity =>
        {
            entity.HasIndex(e => e.FlowId, "IX_FlowStarterTasks_FlowId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Flow).WithMany(p => p.FlowStarterTasks)
                .HasForeignKey(d => d.FlowId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.FlowStarterTask).HasForeignKey<FlowStarterTask>(d => d.Id);
        });

        modelBuilder.Entity<FlowStep>(entity =>
        {
            entity.HasIndex(e => e.FlowInstanceId, "IX_FlowSteps_FlowInstanceId");

            entity.HasIndex(e => new { e.FlowInstanceId, e.StatusId }, "IX_FlowSteps_FlowInstanceId_StatusId_AA805");

            entity.HasIndex(e => new { e.FlowInstanceId, e.StatusId }, "IX_FlowSteps_FlowInstanceId_StatusId_BA90C");

            entity.HasIndex(e => e.FlowNodeId, "IX_FlowSteps_FlowNodeId");

            entity.HasIndex(e => e.FlowNodeId, "IX_FlowSteps_FlowNodeId_00094");

            entity.HasIndex(e => new { e.FlowNodeId, e.OwnerId }, "IX_FlowSteps_FlowNodeId_OwnerId_6A8D9");

            entity.HasIndex(e => new { e.FlowNodeId, e.StatusId }, "IX_FlowSteps_FlowNodeId_StatusId_52378");

            entity.HasIndex(e => new { e.FlowNodeId, e.StatusId }, "IX_FlowSteps_FlowNodeId_StatusId_6AE95");

            entity.HasIndex(e => new { e.FlowNodeId, e.StatusId, e.CreatedAt }, "IX_FlowSteps_FlowNodeId_StatusId_CreatedAt_8BFCA");

            entity.HasIndex(e => e.OwnerId, "IX_FlowSteps_OwnerId");

            entity.HasIndex(e => e.OwnerId, "IX_FlowSteps_OwnerId_E4853");

            entity.HasIndex(e => e.StatusId, "IX_FlowSteps_StatusId");

            entity.HasIndex(e => new { e.StatusId, e.OwnerId }, "IX_FlowSteps_StatusId_OwnerId_FA67C");

            entity.HasIndex(e => e.FlowNodeId, "idx_FlowNodeId");

            entity.HasIndex(e => new { e.StatusId, e.OwnerId }, "idx_StatusId_OwnerId");

            entity.Property(e => e.SubTitle).HasMaxLength(128);
            entity.Property(e => e.Title).HasMaxLength(512);

            entity.HasOne(d => d.FlowInstance).WithMany(p => p.FlowSteps).HasForeignKey(d => d.FlowInstanceId);

            entity.HasOne(d => d.FlowNode).WithMany(p => p.FlowSteps)
                .HasForeignKey(d => d.FlowNodeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Owner).WithMany(p => p.FlowSteps).HasForeignKey(d => d.OwnerId);

            entity.HasOne(d => d.Status).WithMany(p => p.FlowSteps).HasForeignKey(d => d.StatusId);
        });

        modelBuilder.Entity<FlowStepStatus>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<FlowVersion>(entity =>
        {
            entity.HasIndex(e => e.FlowId, "IX_FlowVersions_FlowId");

            entity.HasOne(d => d.Flow).WithMany(p => p.FlowVersions).HasForeignKey(d => d.FlowId);
        });

        modelBuilder.Entity<FormEditTask>(entity =>
        {
            entity.HasIndex(e => e.EditViewId, "IX_FormEditTasks_EditViewId");

            entity.HasIndex(e => e.ViewRecordViewId, "IX_FormEditTasks_ViewRecordViewId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.EditUrl).HasMaxLength(2048);
            entity.Property(e => e.ViewRecordUrl).HasMaxLength(2048);

            entity.HasOne(d => d.EditView).WithMany(p => p.FormEditTaskEditViews).HasForeignKey(d => d.EditViewId);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.FormEditTask).HasForeignKey<FormEditTask>(d => d.Id);

            entity.HasOne(d => d.ViewRecordView).WithMany(p => p.FormEditTaskViewRecordViews).HasForeignKey(d => d.ViewRecordViewId);
        });

        modelBuilder.Entity<FormEntryTask>(entity =>
        {
            entity.HasIndex(e => e.AddViewId, "IX_FormEntryTasks_AddViewId");

            entity.HasIndex(e => e.EditViewId, "IX_FormEntryTasks_EditViewId");

            entity.HasIndex(e => e.ViewRecordViewId, "IX_FormEntryTasks_ViewRecordViewId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AddUrl).HasMaxLength(2048);
            entity.Property(e => e.EditUrl).HasMaxLength(2048);
            entity.Property(e => e.ViewRecordUrl).HasMaxLength(2048);

            entity.HasOne(d => d.AddView).WithMany(p => p.FormEntryTaskAddViews).HasForeignKey(d => d.AddViewId);

            entity.HasOne(d => d.EditView).WithMany(p => p.FormEntryTaskEditViews).HasForeignKey(d => d.EditViewId);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.FormEntryTask).HasForeignKey<FormEntryTask>(d => d.Id);

            entity.HasOne(d => d.ViewRecordView).WithMany(p => p.FormEntryTaskViewRecordViews).HasForeignKey(d => d.ViewRecordViewId);
        });

        modelBuilder.Entity<GasBranchOffice>(entity =>
        {
            entity.ToTable("GasBranchOffices", "cedo");

            entity.HasIndex(e => e.CityId, "IX_GasBranchOffices_CityId");

            entity.Property(e => e.Address).HasMaxLength(512);
            entity.Property(e => e.Telephone).HasMaxLength(20);
            entity.Property(e => e.Title).HasMaxLength(256);

            entity.HasOne(d => d.City).WithMany(p => p.GasBranchOffices).HasForeignKey(d => d.CityId);
        });

        modelBuilder.Entity<GasBranchUser>(entity =>
        {
            entity.ToTable("GasBranchUsers", "cedo");

            entity.HasIndex(e => e.BranchId, "IX_GasBranchUsers_BranchId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Branch).WithMany(p => p.GasBranchUsers).HasForeignKey(d => d.BranchId);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.GasBranchUser).HasForeignKey<GasBranchUser>(d => d.Id);
        });

        modelBuilder.Entity<GasCounterType>(entity =>
        {
            entity.ToTable("GasCounterTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<GasOncallStatus>(entity =>
        {
            entity.ToTable("GasOncallStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<GasRequest>(entity =>
        {
            entity.ToTable("GasRequests", "cedo");

            entity.HasIndex(e => e.CityId, "IX_GasRequests_CityId");

            entity.HasIndex(e => e.OwnershipTypeId, "IX_GasRequests_OwnershipTypeId");

            entity.HasIndex(e => e.RequestId, "IX_GasRequests_RequestId");

            entity.HasIndex(e => e.ScopeId, "IX_GasRequests_ScopeId");

            entity.HasIndex(e => e.StatusId, "IX_GasRequests_StatusId");

            entity.HasIndex(e => e.TypeId, "IX_GasRequests_TypeId");

            entity.Property(e => e.Address).HasMaxLength(512);
            entity.Property(e => e.AddressRegion).HasMaxLength(128);
            entity.Property(e => e.DossierNumber).HasMaxLength(128);
            entity.Property(e => e.FlagSerial).HasMaxLength(50);
            entity.Property(e => e.FreeRequestComment).HasMaxLength(2048);
            entity.Property(e => e.StatusId).HasDefaultValue(1);
            entity.Property(e => e.TrackCode).HasMaxLength(128);

            entity.HasOne(d => d.City).WithMany(p => p.GasRequests).HasForeignKey(d => d.CityId);

            entity.HasOne(d => d.OwnershipType).WithMany(p => p.GasRequests).HasForeignKey(d => d.OwnershipTypeId);

            entity.HasOne(d => d.Request).WithMany(p => p.GasRequests).HasForeignKey(d => d.RequestId);

            entity.HasOne(d => d.Scope).WithMany(p => p.GasRequests).HasForeignKey(d => d.ScopeId);

            entity.HasOne(d => d.Status).WithMany(p => p.GasRequests).HasForeignKey(d => d.StatusId);

            entity.HasOne(d => d.Type).WithMany(p => p.GasRequests).HasForeignKey(d => d.TypeId);
        });

        modelBuilder.Entity<GasRequestAuditForm>(entity =>
        {
            entity.ToTable("GasRequestAuditForms", "cedo");

            entity.HasIndex(e => e.CounterTypeId, "IX_GasRequestAuditForms_CounterTypeId");

            entity.HasIndex(e => e.UsageTypeId, "IX_GasRequestAuditForms_UsageTypeId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ChimneyOrder).HasMaxLength(1024);
            entity.Property(e => e.Gccount).HasColumnName("GCCount");
            entity.Property(e => e.Gcusage).HasColumnName("GCUsage");
            entity.Property(e => e.Gmlocated).HasColumnName("GMLocated");
            entity.Property(e => e.GmlocationDescription)
                .HasMaxLength(512)
                .HasColumnName("GMLocationDescription");
            entity.Property(e => e.HatchDimention).HasMaxLength(512);
            entity.Property(e => e.Hchimney).HasColumnName("HChimney");
            entity.Property(e => e.Hcount).HasColumnName("HCount");
            entity.Property(e => e.Husage).HasColumnName("HUsage");
            entity.Property(e => e.Licount).HasColumnName("LICount");
            entity.Property(e => e.Liusage).HasColumnName("LIUsage");
            entity.Property(e => e.Rccount).HasColumnName("RCCount");
            entity.Property(e => e.Rcusage).HasColumnName("RCUsage");
            entity.Property(e => e.Units).HasDefaultValue(1);
            entity.Property(e => e.WhgroundChimney).HasColumnName("WHGroundChimney");
            entity.Property(e => e.WhgroundCount).HasColumnName("WHGroundCount");
            entity.Property(e => e.WhgroundUsage).HasColumnName("WHGroundUsage");
            entity.Property(e => e.WhwallChimney).HasColumnName("WHWallChimney");
            entity.Property(e => e.WhwallCount).HasColumnName("WHWallCount");
            entity.Property(e => e.WhwallUsage).HasColumnName("WHWallUsage");

            entity.HasOne(d => d.CounterType).WithMany(p => p.GasRequestAuditForms)
                .HasForeignKey(d => d.CounterTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.GasRequestAuditForm).HasForeignKey<GasRequestAuditForm>(d => d.Id);

            entity.HasOne(d => d.UsageType).WithMany(p => p.GasRequestAuditForms)
                .HasForeignKey(d => d.UsageTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<GasRequestInvolvedMember>(entity =>
        {
            entity.ToTable("GasRequestInvolvedMembers", "cedo");

            entity.HasIndex(e => e.MemberServiceId, "IX_GasRequestInvolvedMembers_MemberServiceId");

            entity.HasIndex(e => e.OncallId, "IX_GasRequestInvolvedMembers_OncallId");

            entity.HasIndex(e => e.RequestId, "IX_GasRequestInvolvedMembers_RequestId");

            entity.HasIndex(e => e.StatusId, "IX_GasRequestInvolvedMembers_StatusId");

            entity.HasOne(d => d.MemberService).WithMany(p => p.GasRequestInvolvedMembers)
                .HasForeignKey(d => d.MemberServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Oncall).WithMany(p => p.GasRequestInvolvedMembers).HasForeignKey(d => d.OncallId);

            entity.HasOne(d => d.Request).WithMany(p => p.GasRequestInvolvedMembers).HasForeignKey(d => d.RequestId);

            entity.HasOne(d => d.Status).WithMany(p => p.GasRequestInvolvedMembers).HasForeignKey(d => d.StatusId);
        });

        modelBuilder.Entity<GasRequestInvolvedMemberChangeLog>(entity =>
        {
            entity.ToTable("GasRequestInvolvedMemberChangeLogs", "cedo");

            entity.HasIndex(e => e.UserId, "IX_GasRequestInvolvedMemberChangeLogs_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(512);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.GasRequestInvolvedMemberChangeLog).HasForeignKey<GasRequestInvolvedMemberChangeLog>(d => d.Id);

            entity.HasOne(d => d.User).WithMany(p => p.GasRequestInvolvedMemberChangeLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<GasRequestOncall>(entity =>
        {
            entity.ToTable("GasRequestOncalls", "cedo");

            entity.HasIndex(e => e.ServiceRequestId, "IX_GasRequestOncalls_ServiceRequestId");

            entity.HasIndex(e => e.StatusId, "IX_GasRequestOncalls_StatusId");

            entity.Property(e => e.Address)
                .HasMaxLength(1024)
                .HasDefaultValue("");
            entity.Property(e => e.OtherReasons).HasMaxLength(512);
            entity.Property(e => e.OwnerMobile)
                .HasMaxLength(11)
                .HasDefaultValue("");
            entity.Property(e => e.OwnerName)
                .HasMaxLength(128)
                .HasDefaultValue("");
            entity.Property(e => e.ReporterMobile).HasMaxLength(11);
            entity.Property(e => e.ReporterName).HasMaxLength(50);
            entity.Property(e => e.Subscription)
                .HasMaxLength(30)
                .HasDefaultValue("");
            entity.Property(e => e.TrackCode)
                .HasMaxLength(128)
                .HasDefaultValue("");

            entity.HasOne(d => d.ServiceRequest).WithMany(p => p.GasRequestOncalls).HasForeignKey(d => d.ServiceRequestId);

            entity.HasOne(d => d.Status).WithMany(p => p.GasRequestOncalls).HasForeignKey(d => d.StatusId);
        });

        modelBuilder.Entity<GasRequestOncallDescription>(entity =>
        {
            entity.ToTable("GasRequestOncallDescriptions", "cedo");

            entity.HasIndex(e => e.GasRequestOncallId, "IX_GasRequestOncallDescriptions_GasRequestOncallId");

            entity.HasIndex(e => e.GasRequestOncallStatusId, "IX_GasRequestOncallDescriptions_GasRequestOncallStatusId");

            entity.Property(e => e.Description)
                .HasMaxLength(1024)
                .HasDefaultValue("");

            entity.HasOne(d => d.GasRequestOncall).WithMany(p => p.GasRequestOncallDescriptions).HasForeignKey(d => d.GasRequestOncallId);

            entity.HasOne(d => d.GasRequestOncallStatus).WithMany(p => p.GasRequestOncallDescriptions).HasForeignKey(d => d.GasRequestOncallStatusId);
        });

        modelBuilder.Entity<GasRequestOncallStatus>(entity =>
        {
            entity.ToTable("GasRequestOncallStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<GasRequestScopeType>(entity =>
        {
            entity.ToTable("GasRequestScopeTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<GasRequestSplit>(entity =>
        {
            entity.ToTable("GasRequestSplits", "cedo");

            entity.HasIndex(e => e.CounterTypeId, "IX_GasRequestSplits_CounterTypeId");

            entity.HasIndex(e => e.FinalCounterTypeId, "IX_GasRequestSplits_FinalCounterTypeId");

            entity.HasIndex(e => e.FinalUsageTypeId, "IX_GasRequestSplits_FinalUsageTypeId");

            entity.HasIndex(e => e.GasRequestId, "IX_GasRequestSplits_GasRequestId");

            entity.HasIndex(e => e.UsageTypeId, "IX_GasRequestSplits_UsageTypeId");

            entity.Property(e => e.Order).HasDefaultValue(1);
            entity.Property(e => e.OwnerMobile).HasMaxLength(11);
            entity.Property(e => e.OwnerName).HasMaxLength(128);
            entity.Property(e => e.OwnerNationalCode).HasMaxLength(10);
            entity.Property(e => e.Subscription).HasMaxLength(50);
            entity.Property(e => e.ZipCode).HasMaxLength(20);

            entity.HasOne(d => d.CounterType).WithMany(p => p.GasRequestSplitCounterTypes).HasForeignKey(d => d.CounterTypeId);

            entity.HasOne(d => d.FinalCounterType).WithMany(p => p.GasRequestSplitFinalCounterTypes).HasForeignKey(d => d.FinalCounterTypeId);

            entity.HasOne(d => d.FinalUsageType).WithMany(p => p.GasRequestSplitFinalUsageTypes).HasForeignKey(d => d.FinalUsageTypeId);

            entity.HasOne(d => d.GasRequest).WithMany(p => p.GasRequestSplits).HasForeignKey(d => d.GasRequestId);

            entity.HasOne(d => d.UsageType).WithMany(p => p.GasRequestSplitUsageTypes).HasForeignKey(d => d.UsageTypeId);
        });

        modelBuilder.Entity<GasRequestSplitDocument>(entity =>
        {
            entity.ToTable("GasRequestSplitDocuments", "cedo");

            entity.HasIndex(e => e.DocumentTypeId, "IX_GasRequestSplitDocuments_DocumentTypeId");

            entity.HasIndex(e => e.GasRequestSplitId, "IX_GasRequestSplitDocuments_GasRequestSplitId");

            entity.Property(e => e.Description).HasMaxLength(512);
            entity.Property(e => e.SheetNumber).HasDefaultValue(1);

            entity.HasOne(d => d.DocumentType).WithMany(p => p.GasRequestSplitDocuments).HasForeignKey(d => d.DocumentTypeId);

            entity.HasOne(d => d.GasRequestSplit).WithMany(p => p.GasRequestSplitDocuments).HasForeignKey(d => d.GasRequestSplitId);
        });

        modelBuilder.Entity<GasRequestSplitProblem>(entity =>
        {
            entity.ToTable("GasRequestSplitProblems", "cedo");

            entity.HasIndex(e => e.SplitId, "IX_GasRequestSplitProblems_SplitId");

            entity.Property(e => e.Description).HasMaxLength(1024);

            entity.HasOne(d => d.Split).WithMany(p => p.GasRequestSplitProblems).HasForeignKey(d => d.SplitId);
        });

        modelBuilder.Entity<GasRequestStatus>(entity =>
        {
            entity.ToTable("GasRequestStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<GasRequestType>(entity =>
        {
            entity.ToTable("GasRequestTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<GasRotatingForm>(entity =>
        {
            entity.ToTable("GasRotatingForms", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_GasRotatingForms_InvolvedMemberId");

            entity.HasIndex(e => e.SubscriptionTypeId, "IX_GasRotatingForms_SubscriptionTypeId");

            entity.Property(e => e.FlagSerial).HasMaxLength(128);
            entity.Property(e => e.PrevSubscription).HasMaxLength(128);
            entity.Property(e => e.ValveType).HasMaxLength(128);

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.GasRotatingForms).HasForeignKey(d => d.InvolvedMemberId);

            entity.HasOne(d => d.SubscriptionType).WithMany(p => p.GasRotatingForms).HasForeignKey(d => d.SubscriptionTypeId);
        });

        modelBuilder.Entity<GasSubscriptionType>(entity =>
        {
            entity.ToTable("GasSubscriptionTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<GasSupervisionResult>(entity =>
        {
            entity.ToTable("GasSupervisionResults", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_GasSupervisionResults_InvolvedMemberId");

            entity.HasIndex(e => e.StateId, "IX_GasSupervisionResults_StateId");

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.GasSupervisionResults).HasForeignKey(d => d.InvolvedMemberId);

            entity.HasOne(d => d.State).WithMany(p => p.GasSupervisionResults).HasForeignKey(d => d.StateId);
        });

        modelBuilder.Entity<GasSupervisionResultState>(entity =>
        {
            entity.ToTable("GasSupervisionResultStates", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<GasUsageType>(entity =>
        {
            entity.ToTable("GasUsageTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<GradeCalculationSetting>(entity =>
        {
            entity.ToTable("GradeCalculationSettings", "cedo");

            entity.HasIndex(e => e.BuildingGroupSettingId, "IX_GradeCalculationSettings_BuildingGroupSettingId");

            entity.HasIndex(e => e.MinGradeId, "IX_GradeCalculationSettings_MinGradeId");

            entity.HasIndex(e => e.ServiceFieldId, "IX_GradeCalculationSettings_ServiceFieldId");

            entity.HasIndex(e => e.ServiceTypeId, "IX_GradeCalculationSettings_ServiceTypeId");

            entity.HasOne(d => d.BuildingGroupSetting).WithMany(p => p.GradeCalculationSettings).HasForeignKey(d => d.BuildingGroupSettingId);

            entity.HasOne(d => d.MinGrade).WithMany(p => p.GradeCalculationSettings).HasForeignKey(d => d.MinGradeId);

            entity.HasOne(d => d.ServiceField).WithMany(p => p.GradeCalculationSettings).HasForeignKey(d => d.ServiceFieldId);

            entity.HasOne(d => d.ServiceType).WithMany(p => p.GradeCalculationSettings).HasForeignKey(d => d.ServiceTypeId);
        });

        modelBuilder.Entity<HealthStatus>(entity =>
        {
            entity.ToTable("HealthStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<HoldingLesson>(entity =>
        {
            entity.ToTable("HoldingLessons", "cedo");

            entity.HasIndex(e => e.CourseHoldingId, "IX_HoldingLessons_CourseHoldingId");

            entity.HasIndex(e => e.CourseLessonId, "IX_HoldingLessons_CourseLessonId");

            entity.HasIndex(e => e.StatusId, "IX_HoldingLessons_StatusId");

            entity.HasIndex(e => e.TeacherMemberServiceId, "IX_HoldingLessons_TeacherMemberServiceId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.CourseHolding).WithMany(p => p.HoldingLessons).HasForeignKey(d => d.CourseHoldingId);

            entity.HasOne(d => d.CourseLesson).WithMany(p => p.HoldingLessons).HasForeignKey(d => d.CourseLessonId);

            entity.HasOne(d => d.Status).WithMany(p => p.HoldingLessons).HasForeignKey(d => d.StatusId);

            entity.HasOne(d => d.TeacherMemberService).WithMany(p => p.HoldingLessons).HasForeignKey(d => d.TeacherMemberServiceId);
        });

        modelBuilder.Entity<HoldingLessonOption>(entity =>
        {
            entity.ToTable("HoldingLessonOptions", "cedo");

            entity.HasIndex(e => e.HoldingLessonId, "IX_HoldingLessonOptions_HoldingLessonId");

            entity.Property(e => e.Title).HasMaxLength(128);

            entity.HasOne(d => d.HoldingLesson).WithMany(p => p.HoldingLessonOptions).HasForeignKey(d => d.HoldingLessonId);
        });

        modelBuilder.Entity<HoldingLessonSession>(entity =>
        {
            entity.ToTable("HoldingLessonSessions", "cedo");

            entity.HasIndex(e => e.HoldingLessonId, "IX_HoldingLessonSessions_HoldingLessonId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.HoldingLesson).WithMany(p => p.HoldingLessonSessions)
                .HasForeignKey(d => d.HoldingLessonId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<HoldingLessonStatus>(entity =>
        {
            entity.ToTable("HoldingLessonStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<IncludedResource>(entity =>
        {
            entity.HasIndex(e => e.ProjectId, "IX_IncludedResources_ProjectId");

            entity.HasIndex(e => e.ResourceTypeId, "IX_IncludedResources_ResourceTypeId");

            entity.Property(e => e.Path).HasMaxLength(1024);

            entity.HasOne(d => d.Project).WithMany(p => p.IncludedResources).HasForeignKey(d => d.ProjectId);

            entity.HasOne(d => d.ResourceType).WithMany(p => p.IncludedResources).HasForeignKey(d => d.ResourceTypeId);
        });

        modelBuilder.Entity<IncludedResourceType>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<InstanceUserGroupMember>(entity =>
        {
            entity.HasIndex(e => e.GroupId, "IX_InstanceUserGroupMembers_GroupId");

            entity.HasIndex(e => e.UserId, "IX_InstanceUserGroupMembers_UserId");

            entity.HasOne(d => d.Group).WithMany(p => p.InstanceUserGroupMembers).HasForeignKey(d => d.GroupId);

            entity.HasOne(d => d.User).WithMany(p => p.InstanceUserGroupMembers).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<IntroductionRequest>(entity =>
        {
            entity.ToTable("IntroductionRequests", "cedo");

            entity.HasIndex(e => e.MemberId, "IX_IntroductionRequests_MemberId");

            entity.HasIndex(e => e.StatusId, "IX_IntroductionRequests_StatusId");

            entity.HasIndex(e => e.TargetId, "IX_IntroductionRequests_TargetId");

            entity.Property(e => e.Description).HasMaxLength(1024);
            entity.Property(e => e.Receiver)
                .HasMaxLength(256)
                .HasDefaultValue("");

            entity.HasOne(d => d.Member).WithMany(p => p.IntroductionRequests).HasForeignKey(d => d.MemberId);

            entity.HasOne(d => d.Status).WithMany(p => p.IntroductionRequests).HasForeignKey(d => d.StatusId);

            entity.HasOne(d => d.Target).WithMany(p => p.IntroductionRequests).HasForeignKey(d => d.TargetId);
        });

        modelBuilder.Entity<IntroductionRequestStatus>(entity =>
        {
            entity.ToTable("IntroductionRequestStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<IntroductionTarget>(entity =>
        {
            entity.ToTable("IntroductionTargets", "cedo");

            entity.Property(e => e.OrgTitle).HasMaxLength(128);
            entity.Property(e => e.ReceiverDescription).HasMaxLength(1024);
            entity.Property(e => e.ReceiverTitle).HasMaxLength(256);
        });

        modelBuilder.Entity<InvolvedMember>(entity =>
        {
            entity.ToTable("InvolvedMembers", "cedo", tb => tb.HasTrigger("InvolvedMembersPreventDelete"));

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_InvolvedMembers_ConstructionLicenseId");

            entity.HasIndex(e => e.InvolvedMemberTypeId, "IX_InvolvedMembers_InvolvedMemberTypeId");

            entity.HasIndex(e => e.MemberServiceId, "IX_InvolvedMembers_MemberServiceId");

            entity.HasIndex(e => e.RegDate, "IX_InvolvedMembers_RegDate_827AA");

            entity.HasIndex(e => e.StatusId, "IX_InvolvedMembers_StatusId");

            entity.HasIndex(e => e.StatusId, "IX_InvolvedMembers_StatusId_7C7DC");

            entity.Property(e => e.InvolvedMemberTypeId).HasDefaultValue(1);

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.InvolvedMembers)
                .HasForeignKey(d => d.ConstructionLicenseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.InvolvedMemberType).WithMany(p => p.InvolvedMembers)
                .HasForeignKey(d => d.InvolvedMemberTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.MemberService).WithMany(p => p.InvolvedMembers)
                .HasForeignKey(d => d.MemberServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Status).WithMany(p => p.InvolvedMembers)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<InvolvedMemberStatus>(entity =>
        {
            entity.ToTable("InvolvedMemberStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<InvolvedMemberType>(entity =>
        {
            entity.ToTable("InvolvedMemberType", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<LegalMember>(entity =>
        {
            entity.ToTable("LegalMembers", "cedo", tb => tb.HasTrigger("LegalMembersPreventDelete"));

            entity.HasIndex(e => e.LegalMemberTypeId, "IX_LegalMembers_LegalMemberTypeId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.RegNumber).HasMaxLength(32);
            entity.Property(e => e.Title).HasMaxLength(128);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.LegalMember)
                .HasForeignKey<LegalMember>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.LegalMemberType).WithMany(p => p.LegalMembers)
                .HasForeignKey(d => d.LegalMemberTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<LegalMemberPersonnel>(entity =>
        {
            entity.ToTable("LegalMemberPersonnels", "cedo");

            entity.HasIndex(e => e.LegalMemberId, "IX_LegalMemberPersonnels_LegalMemberId");

            entity.HasIndex(e => e.MemberId, "IX_LegalMemberPersonnels_MemberId");

            entity.HasIndex(e => e.PostId, "IX_LegalMemberPersonnels_PostId");

            entity.HasOne(d => d.LegalMember).WithMany(p => p.LegalMemberPersonnel)
                .HasForeignKey(d => d.LegalMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Member).WithMany(p => p.LegalMemberPersonnel)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Post).WithMany(p => p.LegalMemberPersonnel).HasForeignKey(d => d.PostId);
        });

        modelBuilder.Entity<LegalMemberType>(entity =>
        {
            entity.ToTable("LegalMemberTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.ToTable("Lessons", "cedo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Code).HasMaxLength(10);
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<LicenseCapacity>(entity =>
        {
            entity.ToTable("LicenseCapacities", "cedo");

            entity.HasIndex(e => e.BuildingGroupId, "IX_LicenseCapacities_BuildingGroupId");

            entity.HasIndex(e => e.MemberId, "IX_LicenseCapacities_MemberId");

            entity.HasIndex(e => e.ServiceFieldId, "IX_LicenseCapacities_ServiceFieldId");

            entity.HasIndex(e => e.ServiceTypeId, "IX_LicenseCapacities_ServiceTypeId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(1024);

            entity.HasOne(d => d.BuildingGroup).WithMany(p => p.LicenseCapacities).HasForeignKey(d => d.BuildingGroupId);

            entity.HasOne(d => d.Member).WithMany(p => p.LicenseCapacities).HasForeignKey(d => d.MemberId);

            entity.HasOne(d => d.ServiceField).WithMany(p => p.LicenseCapacities).HasForeignKey(d => d.ServiceFieldId);

            entity.HasOne(d => d.ServiceType).WithMany(p => p.LicenseCapacities)
                .HasForeignKey(d => d.ServiceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<LicenseCapacityRemaining>(entity =>
        {
            entity.ToTable("LicenseCapacityRemainings", "cedo");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.LicenseCapacityRemaining).HasForeignKey<LicenseCapacityRemaining>(d => d.Id);
        });

        modelBuilder.Entity<LicenseInquery>(entity =>
        {
            entity.ToTable("LicenseInqueries", "cedo");

            entity.HasIndex(e => e.Code, "IX_LicenseInqueries_Code_DFB09");

            entity.HasIndex(e => new { e.Code, e.LicenseNumber }, "IX_LicenseInqueries_Code_LicenseNumber_EE5A9");

            entity.HasIndex(e => e.DossierTypeId, "IX_LicenseInqueries_DossierTypeId");

            entity.HasIndex(e => e.LicenseInqueryStatusId, "IX_LicenseInqueries_LicenseInqueryStatusId");

            entity.HasIndex(e => e.LicenseNumber, "IX_LicenseInqueries_LicenseNumber_102E9");

            entity.HasIndex(e => e.MunicipalityId, "IX_LicenseInqueries_MunicipalityId");

            entity.HasIndex(e => new { e.MunicipalityId, e.Code, e.LicenseInqueryStatusId }, "IX_LicenseInqueries_MunicipalityId_Code_LicenseInqueryStatusId_9815F");

            entity.Property(e => e.Code).HasMaxLength(128);
            entity.Property(e => e.DossierTypeId).HasDefaultValue(1);
            entity.Property(e => e.LicenseInqueryStatusId).HasDefaultValue(1);
            entity.Property(e => e.LicenseNumber).HasMaxLength(128);
            entity.Property(e => e.NationalCode).HasMaxLength(10);
            entity.Property(e => e.OwnerFullName)
                .HasMaxLength(128)
                .HasDefaultValue("");

            entity.HasOne(d => d.DossierType).WithMany(p => p.LicenseInqueries).HasForeignKey(d => d.DossierTypeId);

            entity.HasOne(d => d.LicenseInqueryStatus).WithMany(p => p.LicenseInqueries).HasForeignKey(d => d.LicenseInqueryStatusId);

            entity.HasOne(d => d.Municipality).WithMany(p => p.LicenseInqueries).HasForeignKey(d => d.MunicipalityId);
        });

        modelBuilder.Entity<LicenseInqueryStatus>(entity =>
        {
            entity.ToTable("LicenseInqueryStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ManpowerSkillCard>(entity =>
        {
            entity.ToTable("ManpowerSkillCards", "cedo");

            entity.HasIndex(e => e.CategoryManpowerSkillCardId, "IX_ManpowerSkillCards_CategoryManpowerSkillCardId");

            entity.Property(e => e.Title).HasMaxLength(128);

            entity.HasOne(d => d.CategoryManpowerSkillCard).WithMany(p => p.ManpowerSkillCards).HasForeignKey(d => d.CategoryManpowerSkillCardId);
        });

        modelBuilder.Entity<ManpowerSkillCardValue>(entity =>
        {
            entity.ToTable("ManpowerSkillCardValues", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_ManpowerSkillCardValues_InvolvedMemberId");

            entity.HasIndex(e => e.ManpowerSkillCardId, "IX_ManpowerSkillCardValues_ManpowerSkillCardId");

            entity.Property(e => e.FullName).HasMaxLength(128);
            entity.Property(e => e.SkillCardNumber).HasMaxLength(128);

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.ManpowerSkillCardValues)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ManpowerSkillCard).WithMany(p => p.ManpowerSkillCardValues).HasForeignKey(d => d.ManpowerSkillCardId);
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.ToTable("Materials", "cedo");

            entity.HasIndex(e => e.MaterialCategoryId, "IX_Materials_MaterialCategoryId");

            entity.Property(e => e.Title).HasMaxLength(256);

            entity.HasOne(d => d.MaterialCategory).WithMany(p => p.Materials).HasForeignKey(d => d.MaterialCategoryId);
        });

        modelBuilder.Entity<MaterialCategory>(entity =>
        {
            entity.ToTable("MaterialCategories", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<MaterialValue>(entity =>
        {
            entity.ToTable("MaterialValues", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_MaterialValues_InvolvedMemberId");

            entity.HasIndex(e => e.MaterialId, "IX_MaterialValues_MaterialId");

            entity.Property(e => e.Description).HasMaxLength(1024);
            entity.Property(e => e.Standard).HasMaxLength(128);
            entity.Property(e => e.StandardNumber).HasMaxLength(128);
            entity.Property(e => e.Type).HasMaxLength(128);

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.MaterialValues)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Material).WithMany(p => p.MaterialValues).HasForeignKey(d => d.MaterialId);
        });

        modelBuilder.Entity<MechanicSafetyForm>(entity =>
        {
            entity.ToTable("MechanicSafetyForms", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_MechanicSafetyForms_ConstructionLicenseId");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_MechanicSafetyForms_InvolvedMemberId");

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.MechanicSafetyForms).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.MechanicSafetyForms).HasForeignKey(d => d.InvolvedMemberId);
        });

        modelBuilder.Entity<MechanicalExecutiveOperationsForm>(entity =>
        {
            entity.ToTable("MechanicalExecutiveOperationsForms", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_MechanicalExecutiveOperationsForms_InvolvedMemberId");

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.MechanicalExecutiveOperationsForms)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<MechanicalPlanControlForm>(entity =>
        {
            entity.ToTable("MechanicalPlanControlForms", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_MechanicalPlanControlForms_InvolvedMemberId");

            entity.Property(e => e.Item14).HasColumnName("item14");

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.MechanicalPlanControlForms)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<MechanicalRedLinesForm>(entity =>
        {
            entity.ToTable("MechanicalRedLinesForms", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_MechanicalRedLinesForms_InvolvedMemberId");

            entity.Property(e => e.Description).HasMaxLength(1024);

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.MechanicalRedLinesForms)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<MechanicalSpecForm>(entity =>
        {
            entity.ToTable("MechanicalSpecForms", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_MechanicalSpecForms_InvolvedMemberId");

            entity.Property(e => e.MechanicalQuality).HasDefaultValue(true);
            entity.Property(e => e.StandardMechanicalMaterials).HasDefaultValue(true);

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.MechanicalSpecForms)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.ToTable("Members", "cedo");

            entity.HasIndex(e => new { e.ExpireDate, e.MemberShipTypeId }, "IX_Members_ExpireDate_MemberShipTypeId_70D63");

            entity.HasIndex(e => new { e.ExpireDate, e.MemberShipTypeId }, "IX_Members_ExpireDate_MemberShipTypeId_861BA");

            entity.HasIndex(e => e.MemberShipTypeId, "IX_Members_MemberShipTypeId");

            entity.HasIndex(e => e.MemberTypeId, "IX_Members_MemberTypeId");

            entity.HasIndex(e => e.MemberTypeId, "IX_Members_MemberTypeId_AE77E");

            entity.HasIndex(e => e.MemberTypeId, "IX_Members_MemberTypeId_DAB79");

            entity.HasIndex(e => new { e.MemberTypeId, e.ExpireDate, e.MemberShipTypeId }, "IX_Members_MemberTypeId_ExpireDate_MemberShipTypeId_E47B7");

            entity.HasIndex(e => new { e.MemberTypeId, e.MemberShipTypeId }, "IX_Members_MemberTypeId_MemberShipTypeId_10FD6");

            entity.HasIndex(e => new { e.MemberTypeId, e.SyncCode }, "IX_Members_MemberTypeId_SyncCode_8C7F3");

            entity.HasIndex(e => e.MembershipCode, "IX_Members_MembershipCode_29379");

            entity.HasIndex(e => new { e.MembershipCode, e.Id }, "IX_Members_MembershipCode_Id_E5D6D");

            entity.HasIndex(e => e.UserId, "IX_Members_UserId");

            entity.Property(e => e.MemberShipTypeId).HasDefaultValue(1);
            entity.Property(e => e.MembershipCode).HasMaxLength(32);

            entity.HasOne(d => d.MemberShipType).WithMany(p => p.Members).HasForeignKey(d => d.MemberShipTypeId);

            entity.HasOne(d => d.MemberType).WithMany(p => p.Members).HasForeignKey(d => d.MemberTypeId);

            entity.HasOne(d => d.User).WithMany(p => p.Members).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<MemberBankAcount>(entity =>
        {
            entity.ToTable("MemberBankAcounts", "cedo");

            entity.HasIndex(e => e.BankAcountTypeId, "IX_MemberBankAcounts_BankAcountTypeId");

            entity.HasIndex(e => e.MemberId, "IX_MemberBankAcounts_MemberId");

            entity.HasOne(d => d.BankAcountType).WithMany(p => p.MemberBankAcounts).HasForeignKey(d => d.BankAcountTypeId);

            entity.HasOne(d => d.Member).WithMany(p => p.MemberBankAcounts).HasForeignKey(d => d.MemberId);
        });

        modelBuilder.Entity<MemberCapacity>(entity =>
        {
            entity.ToTable("MemberCapacities", "cedo");

            entity.HasIndex(e => e.CityId, "IX_MemberCapacities_CityId");

            entity.HasIndex(e => e.FieldId, "IX_MemberCapacities_FieldId");

            entity.HasIndex(e => e.MemberId, "IX_MemberCapacities_MemberId");

            entity.HasIndex(e => e.ServiceTypeId, "IX_MemberCapacities_ServiceTypeId");

            entity.Property(e => e.Year).HasDefaultValue(1400);

            entity.HasOne(d => d.City).WithMany(p => p.MemberCapacities).HasForeignKey(d => d.CityId);

            entity.HasOne(d => d.Field).WithMany(p => p.MemberCapacities).HasForeignKey(d => d.FieldId);

            entity.HasOne(d => d.Member).WithMany(p => p.MemberCapacities).HasForeignKey(d => d.MemberId);

            entity.HasOne(d => d.ServiceType).WithMany(p => p.MemberCapacities).HasForeignKey(d => d.ServiceTypeId);
        });

        modelBuilder.Entity<MemberCapacityDetail>(entity =>
        {
            entity.ToTable("MemberCapacityDetails", "cedo");

            entity.HasIndex(e => e.BuildingSubGroupId, "IX_MemberCapacityDetails_BuildingSubGroupId");

            entity.HasIndex(e => e.MemberServiceId, "IX_MemberCapacityDetails_MemberServiceId");

            entity.HasIndex(e => new { e.MemberServiceId, e.BuildingSubGroupId, e.RegDate }, "IX_MemberCapacityDetails_MemberServiceId_BuildingSubGroupId_RegDate_5779F");

            entity.HasIndex(e => new { e.MemberServiceId, e.RegDate }, "IX_MemberCapacityDetails_MemberServiceId_RegDate_8719F");

            entity.Property(e => e.BuildingSubGroupId).HasDefaultValue(1);
            entity.Property(e => e.Description).HasMaxLength(1000);

            entity.HasOne(d => d.BuildingSubGroup).WithMany(p => p.MemberCapacityDetails).HasForeignKey(d => d.BuildingSubGroupId);

            entity.HasOne(d => d.MemberService).WithMany(p => p.MemberCapacityDetails).HasForeignKey(d => d.MemberServiceId);
        });

        modelBuilder.Entity<MemberContact>(entity =>
        {
            entity.ToTable("MemberContacts", "cedo");

            entity.HasIndex(e => e.ContactTypeId, "IX_MemberContacts_ContactTypeId");

            entity.HasIndex(e => e.ContactTypeId, "IX_MemberContacts_ContactTypeId_9C3BD");

            entity.HasIndex(e => e.MemberId, "IX_MemberContacts_MemberId");

            entity.Property(e => e.Data).HasMaxLength(256);

            entity.HasOne(d => d.ContactType).WithMany(p => p.MemberContacts).HasForeignKey(d => d.ContactTypeId);

            entity.HasOne(d => d.Member).WithMany(p => p.MemberContacts).HasForeignKey(d => d.MemberId);
        });

        modelBuilder.Entity<MemberService>(entity =>
        {
            entity.ToTable("MemberServices", "cedo");

            entity.HasIndex(e => e.ActivityLicenseId, "IX_MemberServices_ActivityLicenseId");

            entity.HasIndex(e => e.CityId, "IX_MemberServices_CityId");

            entity.HasIndex(e => e.CityId, "IX_MemberServices_CityId_29C44");

            entity.HasIndex(e => e.GradeId, "IX_MemberServices_GradeId");

            entity.HasIndex(e => e.GradeId, "IX_MemberServices_GradeId_93277");

            entity.HasIndex(e => e.ServiceFieldId, "IX_MemberServices_ServiceFieldId");

            entity.HasIndex(e => e.ServiceFieldId, "IX_MemberServices_ServiceFieldId_8486C");

            entity.HasIndex(e => e.ServiceTypeId, "IX_MemberServices_ServiceTypeId");

            entity.HasIndex(e => e.ServiceTypeId, "IX_MemberServices_ServiceTypeId_864F5");

            entity.HasIndex(e => new { e.ServiceTypeId, e.CityId, e.IsActive, e.GradeId }, "IX_MemberServices_ServiceTypeId_CityId_IsActive_GradeId_D24A5");

            entity.HasIndex(e => new { e.ServiceTypeId, e.CityId, e.IsActive, e.ServiceFieldId, e.GradeId }, "IX_MemberServices_ServiceTypeId_CityId_IsActive_ServiceFieldId_GradeId_9136E");

            entity.HasIndex(e => new { e.ServiceTypeId, e.ServiceFieldId, e.IsActive, e.GradeId }, "IX_MemberServices_ServiceTypeId_ServiceFieldId_IsActive_GradeId_B59AE");

            entity.HasOne(d => d.ActivityLicense).WithMany(p => p.MemberServices).HasForeignKey(d => d.ActivityLicenseId);

            entity.HasOne(d => d.City).WithMany(p => p.MemberServices)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Grade).WithMany(p => p.MemberServices).HasForeignKey(d => d.GradeId);

            entity.HasOne(d => d.ServiceField).WithMany(p => p.MemberServices).HasForeignKey(d => d.ServiceFieldId);

            entity.HasOne(d => d.ServiceType).WithMany(p => p.MemberServices)
                .HasForeignKey(d => d.ServiceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<MemberShipType>(entity =>
        {
            entity.ToTable("MemberShipTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<MemberType>(entity =>
        {
            entity.ToTable("MemberTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<MemberWorkField>(entity =>
        {
            entity.ToTable("MemberWorkFields", "cedo");

            entity.HasIndex(e => e.MemberId, "IX_MemberWorkFields_MemberId");

            entity.HasIndex(e => e.ServiceFieldId, "IX_MemberWorkFields_ServiceFieldId");

            entity.HasOne(d => d.Member).WithMany(p => p.MemberWorkFields).HasForeignKey(d => d.MemberId);

            entity.HasOne(d => d.ServiceField).WithMany(p => p.MemberWorkFields)
                .HasForeignKey(d => d.ServiceFieldId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasIndex(e => e.ParentMenuItemId, "IX_MenuItems_ParentMenuItemId");

            entity.HasIndex(e => e.ProjectMenuId, "IX_MenuItems_ProjectMenuId");

            entity.Property(e => e.IconClass).HasMaxLength(128);
            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Url).HasMaxLength(2048);

            entity.HasOne(d => d.ParentMenuItem).WithMany(p => p.InverseParentMenuItem).HasForeignKey(d => d.ParentMenuItemId);

            entity.HasOne(d => d.ProjectMenu).WithMany(p => p.MenuItems).HasForeignKey(d => d.ProjectMenuId);
        });

        modelBuilder.Entity<MenuItemExternalLinkSetting>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Url).HasMaxLength(1024);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.MenuItemExternalLinkSetting).HasForeignKey<MenuItemExternalLinkSetting>(d => d.Id);
        });

        modelBuilder.Entity<MenuItemFlowLinkSetting>(entity =>
        {
            entity.HasIndex(e => e.FlowId, "IX_MenuItemFlowLinkSettings_FlowId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Flow).WithMany(p => p.MenuItemFlowLinkSettings)
                .HasForeignKey(d => d.FlowId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.MenuItemFlowLinkSetting).HasForeignKey<MenuItemFlowLinkSetting>(d => d.Id);
        });

        modelBuilder.Entity<MenuItemViewLinkSetting>(entity =>
        {
            entity.HasIndex(e => e.EntityViewId, "IX_MenuItemViewLinkSettings_EntityViewId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.EntityView).WithMany(p => p.MenuItemViewLinkSettings)
                .HasForeignKey(d => d.EntityViewId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.MenuItemViewLinkSetting).HasForeignKey<MenuItemViewLinkSetting>(d => d.Id);
        });

        modelBuilder.Entity<MessageCatchEvent>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.MessageName).HasMaxLength(200);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.MessageCatchEvent).HasForeignKey<MessageCatchEvent>(d => d.Id);
        });

        modelBuilder.Entity<MessageThrowEvent>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.MessageName)
                .HasMaxLength(200)
                .HasDefaultValue("");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.MessageThrowEvent).HasForeignKey<MessageThrowEvent>(d => d.Id);
        });

        modelBuilder.Entity<Municipality>(entity =>
        {
            entity.ToTable("Municipalities", "cedo");

            entity.HasIndex(e => e.CityId, "IX_Municipalities_CityId");

            entity.Property(e => e.RegionTitle).HasMaxLength(128);
            entity.Property(e => e.Title).HasMaxLength(128);

            entity.HasOne(d => d.City).WithMany(p => p.Municipalities).HasForeignKey(d => d.CityId);
        });

        modelBuilder.Entity<NodeAnnotationLayout>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.NodeAnnotationLayout).HasForeignKey<NodeAnnotationLayout>(d => d.Id);
        });

        modelBuilder.Entity<NodeShapeStyle>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.NodeShapeStyle).HasForeignKey<NodeShapeStyle>(d => d.Id);
        });

        modelBuilder.Entity<NodeTask>(entity =>
        {
            entity.HasIndex(e => e.TaskTypeId, "IX_NodeTasks_TaskTypeId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.NodeTask).HasForeignKey<NodeTask>(d => d.Id);

            entity.HasOne(d => d.TaskType).WithMany(p => p.NodeTasks).HasForeignKey(d => d.TaskTypeId);
        });

        modelBuilder.Entity<OnlinePaymentSetting>(entity =>
        {
            entity.ToTable("OnlinePaymentSettings", "cedo");

            entity.HasIndex(e => e.BillTypeId, "IX_OnlinePaymentSettings_BillTypeId");

            entity.HasIndex(e => e.CityId, "IX_OnlinePaymentSettings_CityId");

            entity.Property(e => e.LoginAccount).HasMaxLength(128);

            entity.HasOne(d => d.BillType).WithMany(p => p.OnlinePaymentSettings)
                .HasForeignKey(d => d.BillTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.City).WithMany(p => p.OnlinePaymentSettings).HasForeignKey(d => d.CityId);
        });

        modelBuilder.Entity<Operator>(entity =>
        {
            entity.ToTable("Operators", "cedo");

            entity.HasIndex(e => e.CityId, "IX_Operators_CityId");

            entity.HasIndex(e => e.OperatorTypeId, "IX_Operators_OperatorTypeId");

            entity.HasIndex(e => e.UserId, "IX_Operators_UserId");

            entity.HasOne(d => d.City).WithMany(p => p.Operators).HasForeignKey(d => d.CityId);

            entity.HasOne(d => d.OperatorType).WithMany(p => p.Operators).HasForeignKey(d => d.OperatorTypeId);

            entity.HasOne(d => d.User).WithMany(p => p.Operators).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Operator1>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Operators");

            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.UserName).HasMaxLength(256);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<OperatorType>(entity =>
        {
            entity.ToTable("OperatorTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<OrgPersonnel>(entity =>
        {
            entity.ToTable("OrgPersonnels", "cedo");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.OrgPersonnel).HasForeignKey<OrgPersonnel>(d => d.Id);
        });

        modelBuilder.Entity<OrgPost>(entity =>
        {
            entity.ToTable("OrgPosts", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<OrgUnit>(entity =>
        {
            entity.ToTable("OrgUnits", "cedo");

            entity.HasIndex(e => e.AgencyId, "IX_OrgUnits_AgencyId");

            entity.HasIndex(e => e.ParentUnitId, "IX_OrgUnits_ParentUnitId");

            entity.HasIndex(e => e.UnitTypeId, "IX_OrgUnits_UnitTypeId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Title).HasMaxLength(128);

            entity.HasOne(d => d.Agency).WithMany(p => p.OrgUnits).HasForeignKey(d => d.AgencyId);

            entity.HasOne(d => d.ParentUnit).WithMany(p => p.InverseParentUnit).HasForeignKey(d => d.ParentUnitId);

            entity.HasOne(d => d.UnitType).WithMany(p => p.OrgUnits).HasForeignKey(d => d.UnitTypeId);
        });

        modelBuilder.Entity<OrgUnitPersonnel>(entity =>
        {
            entity.ToTable("OrgUnitPersonnels", "cedo");

            entity.HasIndex(e => e.PersonnelId, "IX_OrgUnitPersonnels_PersonnelId");

            entity.HasIndex(e => e.PostId, "IX_OrgUnitPersonnels_PostId");

            entity.HasIndex(e => e.UnitId, "IX_OrgUnitPersonnels_UnitId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Personnel).WithMany(p => p.OrgUnitPersonnel).HasForeignKey(d => d.PersonnelId);

            entity.HasOne(d => d.Post).WithMany(p => p.OrgUnitPersonnel).HasForeignKey(d => d.PostId);

            entity.HasOne(d => d.Unit).WithMany(p => p.OrgUnitPersonnel).HasForeignKey(d => d.UnitId);
        });

        modelBuilder.Entity<OrgUnitType>(entity =>
        {
            entity.ToTable("OrgUnitTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<OrganizationGroup>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<OrganizationPost>(entity =>
        {
            entity.HasIndex(e => e.ParentId, "IX_OrganizationPosts_ParentId");

            entity.Property(e => e.Title).HasMaxLength(128);

            entity.HasOne(d => d.Parent).WithMany(p => p.OrganizationPosts).HasForeignKey(d => d.ParentId);
        });

        modelBuilder.Entity<OrganizationUnit>(entity =>
        {
            entity.HasIndex(e => e.ParentId, "IX_OrganizationUnits_ParentId");

            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasForeignKey(d => d.ParentId);
        });

        modelBuilder.Entity<Owner>(entity =>
        {
            entity.ToTable("Owners", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_Owners_ConstructionLicenseId");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_Owners_ConstructionLicenseId_AD8EE");

            entity.HasIndex(e => e.NationalCode, "IX_Owners_NationalCode_0E482");

            entity.HasIndex(e => e.OwnerTypeId, "IX_Owners_OwnerTypeId");

            entity.HasIndex(e => e.UserId, "IX_Owners_UserId");

            entity.Property(e => e.AttorneyNumber).HasMaxLength(20);
            entity.Property(e => e.FatherName).HasMaxLength(128);
            entity.Property(e => e.FirstName).HasMaxLength(128);
            entity.Property(e => e.IdNumber)
                .HasMaxLength(20)
                .HasDefaultValue("");
            entity.Property(e => e.IssuePlace).HasMaxLength(128);
            entity.Property(e => e.Job).HasMaxLength(128);
            entity.Property(e => e.LastName).HasMaxLength(128);
            entity.Property(e => e.Mobile).HasMaxLength(20);
            entity.Property(e => e.NationalCode).HasMaxLength(10);
            entity.Property(e => e.OtherOwnerType).HasMaxLength(256);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.ResidenceAddress).HasMaxLength(256);
            entity.Property(e => e.ResidencePostalCode).HasMaxLength(15);
            entity.Property(e => e.SanaCode).HasMaxLength(50);
            entity.Property(e => e.WorkAddress).HasMaxLength(256);

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.Owners).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.OwnerType).WithMany(p => p.Owners).HasForeignKey(d => d.OwnerTypeId);

            entity.HasOne(d => d.User).WithMany(p => p.Owners).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<OwnerType>(entity =>
        {
            entity.ToTable("OwnerTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<OwnershipDocumentType>(entity =>
        {
            entity.ToTable("OwnershipDocumentTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<PageMetaTag>(entity =>
        {
            entity.HasIndex(e => e.ProjectId, "IX_PageMetaTags_ProjectId");

            entity.Property(e => e.Content).HasMaxLength(2048);
            entity.Property(e => e.Name).HasMaxLength(256);

            entity.HasOne(d => d.Project).WithMany(p => p.PageMetaTags).HasForeignKey(d => d.ProjectId);
        });

        modelBuilder.Entity<ParaRole>(entity =>
        {
            entity.ToTable("PARA-Roles");

            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<ParaRoleClaim>(entity =>
        {
            entity.ToTable("PARA-RoleClaims");

            entity.HasIndex(e => e.RoleId, "IX_PARA-RoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.ParaRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<ParaUser>(entity =>
        {
            entity.ToTable("PARA-Users");

            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.UserName, "IX_PARA-Users_UserName_547DB");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "ParaUserRole",
                    r => r.HasOne<ParaRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<ParaUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("PARA-UserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_PARA-UserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<ParaUserClaim>(entity =>
        {
            entity.ToTable("PARA-UserClaims");

            entity.HasIndex(e => e.UserId, "IX_PARA-UserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.ParaUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<ParaUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.ToTable("PARA-UserLogins");

            entity.HasIndex(e => e.UserId, "IX_PARA-UserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.ParaUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<ParaUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.ToTable("PARA-UserTokens");

            entity.HasOne(d => d.User).WithMany(p => p.ParaUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<PasswayType>(entity =>
        {
            entity.ToTable("PasswayTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Payment");

            entity.Property(e => e.DossierNumber).HasMaxLength(64);
            entity.Property(e => e.LicenseNumber).HasMaxLength(64);
            entity.Property(e => e.Rrn)
                .HasMaxLength(128)
                .HasColumnName("RRN");
        });

        modelBuilder.Entity<PaymentComeBackRequest>(entity =>
        {
            entity.ToTable("PaymentComeBackRequests", "cedo");

            entity.HasIndex(e => e.BillId, "IX_PaymentComeBackRequests_BillId");

            entity.HasIndex(e => e.RequestStatusId, "IX_PaymentComeBackRequests_RequestStatusId");

            entity.Property(e => e.AccountNumber).HasMaxLength(128);
            entity.Property(e => e.BankName).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(2048);

            entity.HasOne(d => d.Bill).WithMany(p => p.PaymentComeBackRequests).HasForeignKey(d => d.BillId);

            entity.HasOne(d => d.RequestStatus).WithMany(p => p.PaymentComeBackRequests).HasForeignKey(d => d.RequestStatusId);
        });

        modelBuilder.Entity<PaymentReceipt>(entity =>
        {
            entity.ToTable("PaymentReceipts", "cedo");

            entity.HasIndex(e => e.BillId, "IX_PaymentReceipts_BillId");

            entity.HasIndex(e => e.PaymentStatusId, "IX_PaymentReceipts_PaymentStatusId");

            entity.HasIndex(e => e.PaymentTypeId, "IX_PaymentReceipts_PaymentTypeId");

            entity.Property(e => e.BillPaymentCode).HasMaxLength(128);
            entity.Property(e => e.Comment).HasMaxLength(512);
            entity.Property(e => e.Description).HasMaxLength(1024);
            entity.Property(e => e.HashCardNumber).HasMaxLength(128);
            entity.Property(e => e.PayCode).HasMaxLength(128);
            entity.Property(e => e.PaymentTypeId).HasDefaultValue(1);
            entity.Property(e => e.Rrn)
                .HasMaxLength(128)
                .HasColumnName("RRN");
            entity.Property(e => e.Token).HasMaxLength(128);
            entity.Property(e => e.TrackCode).HasMaxLength(1000);

            entity.HasOne(d => d.Bill).WithMany(p => p.PaymentReceipts).HasForeignKey(d => d.BillId);

            entity.HasOne(d => d.PaymentStatus).WithMany(p => p.PaymentReceipts).HasForeignKey(d => d.PaymentStatusId);

            entity.HasOne(d => d.PaymentType).WithMany(p => p.PaymentReceipts).HasForeignKey(d => d.PaymentTypeId);
        });

        modelBuilder.Entity<PaymentStatus>(entity =>
        {
            entity.ToTable("PaymentStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<PaymentType>(entity =>
        {
            entity.ToTable("PaymentTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<Personel>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.PersonelCode).HasMaxLength(64);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Personel).HasForeignKey<Personel>(d => d.Id);
        });

        modelBuilder.Entity<PersonelDossier>(entity =>
        {
            entity.HasIndex(e => e.PersonelId, "IX_PersonelDossiers_PersonelId");

            entity.HasIndex(e => e.WorkingStatusId, "IX_PersonelDossiers_WorkingStatusId");

            entity.HasOne(d => d.Personel).WithMany(p => p.PersonelDossiers).HasForeignKey(d => d.PersonelId);

            entity.HasOne(d => d.WorkingStatus).WithMany(p => p.PersonelDossiers).HasForeignKey(d => d.WorkingStatusId);
        });

        modelBuilder.Entity<PersonelGroup>(entity =>
        {
            entity.HasIndex(e => e.OrganizationGroupId, "IX_PersonelGroups_OrganizationGroupId");

            entity.HasIndex(e => e.PersonelId, "IX_PersonelGroups_PersonelId");

            entity.HasOne(d => d.OrganizationGroup).WithMany(p => p.PersonelGroups).HasForeignKey(d => d.OrganizationGroupId);

            entity.HasOne(d => d.Personel).WithMany(p => p.PersonelGroups).HasForeignKey(d => d.PersonelId);
        });

        modelBuilder.Entity<PersonelPost>(entity =>
        {
            entity.HasIndex(e => e.OrganizationPostId, "IX_PersonelPosts_OrganizationPostId");

            entity.HasIndex(e => e.PersonelId, "IX_PersonelPosts_PersonelId");

            entity.HasOne(d => d.OrganizationPost).WithMany(p => p.PersonelPosts).HasForeignKey(d => d.OrganizationPostId);

            entity.HasOne(d => d.Personel).WithMany(p => p.PersonelPosts).HasForeignKey(d => d.PersonelId);
        });

        modelBuilder.Entity<PersonnelPost>(entity =>
        {
            entity.ToTable("PersonnelPosts", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<PlanControl>(entity =>
        {
            entity.ToTable("PlanControls", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseDocumentId, "IX_PlanControls_ConstructionLicenseDocumentId");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_PlanControls_InvolvedMemberId");

            entity.HasIndex(e => e.PlanControlStatusId, "IX_PlanControls_PlanControlStatusId");

            entity.HasOne(d => d.ConstructionLicenseDocument).WithMany(p => p.PlanControls)
                .HasForeignKey(d => d.ConstructionLicenseDocumentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.PlanControls)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PlanControlStatus).WithMany(p => p.PlanControls).HasForeignKey(d => d.PlanControlStatusId);
        });

        modelBuilder.Entity<PlanControlSetting>(entity =>
        {
            entity.ToTable("PlanControlSettings", "cedo");

            entity.HasIndex(e => e.BuildingSubGroupId, "IX_PlanControlSettings_BuildingSubGroupId");

            entity.HasIndex(e => e.DefaultUserIdId, "IX_PlanControlSettings_DefaultUserIdId");

            entity.HasIndex(e => e.DossierTypeId, "IX_PlanControlSettings_DossierTypeId");

            entity.HasIndex(e => e.ServiceFieldId, "IX_PlanControlSettings_ServiceFieldId");

            entity.HasIndex(e => e.ServiceTypeId, "IX_PlanControlSettings_ServiceTypeId");

            entity.HasIndex(e => e.SourceCityId, "IX_PlanControlSettings_SourceCityId");

            entity.HasIndex(e => e.TargetCityId, "IX_PlanControlSettings_TargetCityId");

            entity.Property(e => e.ServiceTypeId).HasDefaultValue(1);

            entity.HasOne(d => d.BuildingSubGroup).WithMany(p => p.PlanControlSettings).HasForeignKey(d => d.BuildingSubGroupId);

            entity.HasOne(d => d.DefaultUserId).WithMany(p => p.PlanControlSettings).HasForeignKey(d => d.DefaultUserIdId);

            entity.HasOne(d => d.DossierType).WithMany(p => p.PlanControlSettings).HasForeignKey(d => d.DossierTypeId);

            entity.HasOne(d => d.ServiceField).WithMany(p => p.PlanControlSettings).HasForeignKey(d => d.ServiceFieldId);

            entity.HasOne(d => d.ServiceType).WithMany(p => p.PlanControlSettings).HasForeignKey(d => d.ServiceTypeId);

            entity.HasOne(d => d.SourceCity).WithMany(p => p.PlanControlSettingSourceCities).HasForeignKey(d => d.SourceCityId);

            entity.HasOne(d => d.TargetCity).WithMany(p => p.PlanControlSettingTargetCities).HasForeignKey(d => d.TargetCityId);
        });

        modelBuilder.Entity<PlanControlStatus>(entity =>
        {
            entity.ToTable("PlanControlStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<PreviousStep>(entity =>
        {
            entity.HasIndex(e => e.PrevStepId, "IX_PreviousSteps_PrevStepId");

            entity.HasIndex(e => e.StepId, "IX_PreviousSteps_StepId");

            entity.HasOne(d => d.PrevStep).WithMany(p => p.PreviousStepPrevSteps)
                .HasForeignKey(d => d.PrevStepId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Step).WithMany(p => p.PreviousStepSteps).HasForeignKey(d => d.StepId);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.Property(e => e.DbSchema).HasMaxLength(128);
            entity.Property(e => e.Enabled).HasDefaultValue(true);
            entity.Property(e => e.FriendlyName).HasMaxLength(128);
            entity.Property(e => e.HomePageUrl).HasMaxLength(2048);
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.Namespace).HasMaxLength(128);
            entity.Property(e => e.ProjectVersion).HasMaxLength(64);
            entity.Property(e => e.RouteParam).HasMaxLength(128);
        });

        modelBuilder.Entity<ProjectCodeDirectory>(entity =>
        {
            entity.HasIndex(e => e.ParentDirectoryId, "IX_ProjectCodeDirectories_ParentDirectoryId");

            entity.HasIndex(e => e.ProjectId, "IX_ProjectCodeDirectories_ProjectId");

            entity.Property(e => e.Title).HasMaxLength(256);

            entity.HasOne(d => d.ParentDirectory).WithMany(p => p.InverseParentDirectory).HasForeignKey(d => d.ParentDirectoryId);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectCodeDirectories).HasForeignKey(d => d.ProjectId);
        });

        modelBuilder.Entity<ProjectCodeFile>(entity =>
        {
            entity.HasIndex(e => e.ParentDirectoryId, "IX_ProjectCodeFiles_ParentDirectoryId");

            entity.Property(e => e.Title).HasMaxLength(256);

            entity.HasOne(d => d.ParentDirectory).WithMany(p => p.ProjectCodeFiles).HasForeignKey(d => d.ParentDirectoryId);
        });

        modelBuilder.Entity<ProjectDataSource>(entity =>
        {
            entity.HasIndex(e => e.DataSourceTypeId, "IX_ProjectDataSources_DataSourceTypeId");

            entity.HasIndex(e => e.ProjectId, "IX_ProjectDataSources_ProjectId");

            entity.Property(e => e.DataSourceName).HasMaxLength(128);
            entity.Property(e => e.DataSourceTypeId).HasDefaultValue(1);
            entity.Property(e => e.DefaultPkType).HasMaxLength(128);
            entity.Property(e => e.FriendlyName).HasMaxLength(128);

            entity.HasOne(d => d.DataSourceType).WithMany(p => p.ProjectDataSources).HasForeignKey(d => d.DataSourceTypeId);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectDataSources).HasForeignKey(d => d.ProjectId);
        });

        modelBuilder.Entity<ProjectHelp>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.ProjectHelp).HasForeignKey<ProjectHelp>(d => d.Id);
        });

        modelBuilder.Entity<ProjectLoginSetting>(entity =>
        {
            entity.HasIndex(e => e.TwoFactorModeId, "IX_ProjectLoginSettings_TwoFactorModeId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.ProjectLoginSetting).HasForeignKey<ProjectLoginSetting>(d => d.Id);

            entity.HasOne(d => d.TwoFactorMode).WithMany(p => p.ProjectLoginSettings).HasForeignKey(d => d.TwoFactorModeId);
        });

        modelBuilder.Entity<ProjectMenu>(entity =>
        {
            entity.HasIndex(e => e.ProjectId, "IX_ProjectMenus_ProjectId");

            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectMenus).HasForeignKey(d => d.ProjectId);
        });

        modelBuilder.Entity<ProjectRecentDocument>(entity =>
        {
            entity.HasIndex(e => e.DocId, "IX_ProjectRecentDocuments_DocId");

            entity.HasIndex(e => e.ProjectId, "IX_ProjectRecentDocuments_ProjectId");

            entity.Property(e => e.DocId).HasMaxLength(128);
            entity.Property(e => e.DocTitle)
                .HasMaxLength(512)
                .HasDefaultValue("");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectRecentDocuments).HasForeignKey(d => d.ProjectId);
        });

        modelBuilder.Entity<ProjectTranslation>(entity =>
        {
            entity.HasIndex(e => e.CultureId, "IX_ProjectTranslations_CultureId");

            entity.HasIndex(e => e.LocalizablePhraseId, "IX_ProjectTranslations_LocalizablePhraseId");

            entity.HasIndex(e => e.ProjectId, "IX_ProjectTranslations_ProjectId");

            entity.HasOne(d => d.Culture).WithMany(p => p.ProjectTranslations).HasForeignKey(d => d.CultureId);

            entity.HasOne(d => d.LocalizablePhrase).WithMany(p => p.ProjectTranslations).HasForeignKey(d => d.LocalizablePhraseId);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectTranslations).HasForeignKey(d => d.ProjectId);
        });

        modelBuilder.Entity<ProjectView>(entity =>
        {
            entity.HasIndex(e => e.ProjectId, "IX_ProjectViews_ProjectId");

            entity.HasIndex(e => e.ViewTypeId, "IX_ProjectViews_ViewTypeId");

            entity.Property(e => e.FileName).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectViews).HasForeignKey(d => d.ProjectId);

            entity.HasOne(d => d.ViewType).WithMany(p => p.ProjectViews).HasForeignKey(d => d.ViewTypeId);
        });

        modelBuilder.Entity<ProjectWebPushSetting>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.GcmSenderId).HasMaxLength(1024);
            entity.Property(e => e.ReplyToAddress).HasMaxLength(1024);
            entity.Property(e => e.VapidPrivateKey).HasMaxLength(1024);
            entity.Property(e => e.VapidPublicKey).HasMaxLength(1024);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.ProjectWebPushSetting).HasForeignKey<ProjectWebPushSetting>(d => d.Id);
        });

        modelBuilder.Entity<ProjectWorkItemSetting>(entity =>
        {
            entity.ToTable("ProjectWorkItemSettings", "cedo");

            entity.HasIndex(e => e.CityId, "IX_ProjectWorkItemSettings_CityId");

            entity.HasIndex(e => e.DossierTypeId, "IX_ProjectWorkItemSettings_DossierTypeId");

            entity.HasOne(d => d.City).WithMany(p => p.ProjectWorkItemSettings).HasForeignKey(d => d.CityId);

            entity.HasOne(d => d.DossierType).WithMany(p => p.ProjectWorkItemSettings).HasForeignKey(d => d.DossierTypeId);
        });

        modelBuilder.Entity<PropertyAttribute>(entity =>
        {
            entity.HasIndex(e => e.EntityPropertyId, "IX_PropertyAttributes_EntityPropertyId");

            entity.Property(e => e.AttributeType).HasMaxLength(1024);

            entity.HasOne(d => d.EntityProperty).WithMany(p => p.PropertyAttributes).HasForeignKey(d => d.EntityPropertyId);
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.ToTable("Provinces", "cedo");

            entity.Property(e => e.Name).HasMaxLength(128);
        });

        modelBuilder.Entity<PwaPreCacheAction>(entity =>
        {
            entity.HasIndex(e => e.ActionId, "IX_PwaPreCacheActions_ActionId");

            entity.HasIndex(e => e.SettingId, "IX_PwaPreCacheActions_SettingId");

            entity.HasOne(d => d.Action).WithMany(p => p.PwaPreCacheActions).HasForeignKey(d => d.ActionId);

            entity.HasOne(d => d.Setting).WithMany(p => p.PwaPreCacheActions)
                .HasForeignKey(d => d.SettingId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PwaSetting>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BackgroundColor).HasMaxLength(20);
            entity.Property(e => e.CacheMode).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(512);
            entity.Property(e => e.Direction).HasMaxLength(4);
            entity.Property(e => e.Display).HasMaxLength(20);
            entity.Property(e => e.Language).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.Orientation).HasMaxLength(50);
            entity.Property(e => e.ShortName).HasMaxLength(128);
            entity.Property(e => e.ThemeColor).HasMaxLength(20);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.PwaSetting).HasForeignKey<PwaSetting>(d => d.Id);
        });

        modelBuilder.Entity<PwaShortcutAction>(entity =>
        {
            entity.HasIndex(e => e.ActionId, "IX_PwaShortcutActions_ActionId");

            entity.HasIndex(e => e.SettingId, "IX_PwaShortcutActions_SettingId");

            entity.Property(e => e.Description).HasMaxLength(512);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.Action).WithMany(p => p.PwaShortcutActions).HasForeignKey(d => d.ActionId);

            entity.HasOne(d => d.Setting).WithMany(p => p.PwaShortcutActions)
                .HasForeignKey(d => d.SettingId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<RealMember>(entity =>
        {
            entity.ToTable("RealMembers", "cedo");

            entity.HasIndex(e => e.HealthStatusId, "IX_RealMembers_HealthStatusId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.HealthStatus).WithMany(p => p.RealMembers).HasForeignKey(d => d.HealthStatusId);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.RealMember).HasForeignKey<RealMember>(d => d.Id);
        });

        modelBuilder.Entity<RejectedPlanControl>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("RejectedPlanControls");

            entity.Property(e => e.DossierNumber).HasMaxLength(64);
            entity.Property(e => e.FirstName).HasMaxLength(128);
            entity.Property(e => e.LastName).HasMaxLength(128);
        });

        modelBuilder.Entity<RejectedPlanControlsWithArea>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("RejectedPlanControlsWithArea");

            entity.Property(e => e.DossierNumber).HasMaxLength(64);
            entity.Property(e => e.FirstName).HasMaxLength(128);
            entity.Property(e => e.LastName).HasMaxLength(128);
        });

        modelBuilder.Entity<RequestStatus>(entity =>
        {
            entity.ToTable("RequestStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<RequestType>(entity =>
        {
            entity.ToTable("RequestTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<RoleMenu>(entity =>
        {
            entity.ToTable("RoleMenu");

            entity.HasIndex(e => e.MenuId, "IX_RoleMenu_MenuId");

            entity.HasIndex(e => e.RoleId, "IX_RoleMenu_RoleId");

            entity.HasOne(d => d.Menu).WithMany(p => p.RoleMenus).HasForeignKey(d => d.MenuId);

            entity.HasOne(d => d.Role).WithMany(p => p.RoleMenus).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<RuleDestination>(entity =>
        {
            entity.HasIndex(e => e.RuleId, "IX_RuleDestinations_RuleId");

            entity.Property(e => e.Method).HasMaxLength(10);
            entity.Property(e => e.Protocol).HasMaxLength(10);
            entity.Property(e => e.Url).HasMaxLength(2048);

            entity.HasOne(d => d.Rule).WithMany(p => p.RuleDestinations).HasForeignKey(d => d.RuleId);
        });

        modelBuilder.Entity<RuleDeviceSource>(entity =>
        {
            entity.HasIndex(e => e.RuleId, "IX_RuleDeviceSources_RuleId");

            entity.Property(e => e.DeviceFamily).HasMaxLength(256);
            entity.Property(e => e.OsFamily).HasMaxLength(256);
            entity.Property(e => e.UserAgentFamily).HasMaxLength(256);

            entity.HasOne(d => d.Rule).WithMany(p => p.RuleDeviceSources).HasForeignKey(d => d.RuleId);
        });

        modelBuilder.Entity<RuleIdentitySource>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.RuleIdentitySource).HasForeignKey<RuleIdentitySource>(d => d.Id);
        });

        modelBuilder.Entity<RuleRoleSource>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_RuleRoleSources_RoleId");

            entity.HasIndex(e => e.RuleId, "IX_RuleRoleSources_RuleId");

            entity.HasOne(d => d.Role).WithMany(p => p.RuleRoleSources).HasForeignKey(d => d.RoleId);

            entity.HasOne(d => d.Rule).WithMany(p => p.RuleRoleSources).HasForeignKey(d => d.RuleId);
        });

        modelBuilder.Entity<RuleSubnetSource>(entity =>
        {
            entity.HasIndex(e => e.RuleId, "IX_RuleSubnetSources_RuleId");

            entity.Property(e => e.Subnet).HasMaxLength(15);

            entity.HasOne(d => d.Rule).WithMany(p => p.RuleSubnetSources).HasForeignKey(d => d.RuleId);
        });

        modelBuilder.Entity<RuleTimeSource>(entity =>
        {
            entity.HasIndex(e => e.RuleId, "IX_RuleTimeSources_RuleId");

            entity.HasOne(d => d.Rule).WithMany(p => p.RuleTimeSources).HasForeignKey(d => d.RuleId);
        });

        modelBuilder.Entity<RuleUserSource>(entity =>
        {
            entity.HasIndex(e => e.RuleId, "IX_RuleUserSources_RuleId");

            entity.HasIndex(e => e.UserId, "IX_RuleUserSources_UserId");

            entity.HasOne(d => d.Rule).WithMany(p => p.RuleUserSources).HasForeignKey(d => d.RuleId);

            entity.HasOne(d => d.User).WithMany(p => p.RuleUserSources).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<SafetyReport>(entity =>
        {
            entity.ToTable("SafetyReports", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_SafetyReports_ConstructionLicenseId");

            entity.HasIndex(e => e.ReportFileId, "IX_SafetyReports_ReportFileId");

            entity.HasIndex(e => e.ReportResultId, "IX_SafetyReports_ReportResultId");

            entity.HasIndex(e => e.ReportStatusId, "IX_SafetyReports_ReportStatusId");

            entity.HasIndex(e => e.ReporterMemberId, "IX_SafetyReports_ReporterMemberId");

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.SafetyReports).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.ReportFile).WithMany(p => p.SafetyReports)
                .HasForeignKey(d => d.ReportFileId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ReportResult).WithMany(p => p.SafetyReports).HasForeignKey(d => d.ReportResultId);

            entity.HasOne(d => d.ReportStatus).WithMany(p => p.SafetyReports).HasForeignKey(d => d.ReportStatusId);

            entity.HasOne(d => d.ReporterMember).WithMany(p => p.SafetyReports)
                .HasForeignKey(d => d.ReporterMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SafetyReportResult>(entity =>
        {
            entity.ToTable("SafetyReportResults", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<SafetyReportStatus>(entity =>
        {
            entity.ToTable("SafetyReportStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ScriptExecuteTask>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.ScriptExecuteTask).HasForeignKey<ScriptExecuteTask>(d => d.Id);
        });

        modelBuilder.Entity<SendEmailTask>(entity =>
        {
            entity.HasIndex(e => e.EmailAccountId, "IX_SendEmailTasks_EmailAccountId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.EmailAccount).WithMany(p => p.SendEmailTasks)
                .HasForeignKey(d => d.EmailAccountId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.SendEmailTask).HasForeignKey<SendEmailTask>(d => d.Id);
        });

        modelBuilder.Entity<SendSmsTask>(entity =>
        {
            entity.HasIndex(e => e.SmsAccountId, "IX_SendSmsTasks_SmsAccountId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.SendSmsTask).HasForeignKey<SendSmsTask>(d => d.Id);

            entity.HasOne(d => d.SmsAccount).WithMany(p => p.SendSmsTasks)
                .HasForeignKey(d => d.SmsAccountId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<SentSmse>(entity =>
        {
            entity.HasIndex(e => e.SmsAccountId, "IX_SentSmses_SmsAccountId");

            entity.HasOne(d => d.SmsAccount).WithMany(p => p.SentSmses).HasForeignKey(d => d.SmsAccountId);
        });

        modelBuilder.Entity<Sentence>(entity =>
        {
            entity.ToTable("Sentences", "cedo");

            entity.HasIndex(e => e.MemberId, "IX_Sentences_MemberId");

            entity.HasIndex(e => e.SentenceStatusId, "IX_Sentences_SentenceStatusId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(1024);
            entity.Property(e => e.JudgmentLetterNumber)
                .HasMaxLength(128)
                .HasDefaultValue("");
            entity.Property(e => e.PlaintiffName).HasMaxLength(256);

            entity.HasOne(d => d.Member).WithMany(p => p.Sentences).HasForeignKey(d => d.MemberId);

            entity.HasOne(d => d.SentenceStatus).WithMany(p => p.Sentences).HasForeignKey(d => d.SentenceStatusId);
        });

        modelBuilder.Entity<SentenceCancellation>(entity =>
        {
            entity.ToTable("SentenceCancellations", "cedo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(1024);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.SentenceCancellation).HasForeignKey<SentenceCancellation>(d => d.Id);
        });

        modelBuilder.Entity<SentenceStatus>(entity =>
        {
            entity.ToTable("SentenceStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<SeparationExchangeFile>(entity =>
        {
            entity.ToTable("SeparationExchangeFiles", "cedo");

            entity.HasIndex(e => e.RequestId, "IX_SeparationExchangeFiles_RequestId");

            entity.HasOne(d => d.Request).WithMany(p => p.SeparationExchangeFiles).HasForeignKey(d => d.RequestId);
        });

        modelBuilder.Entity<SeparationInvolvedMember>(entity =>
        {
            entity.ToTable("SeparationInvolvedMembers", "cedo");

            entity.HasIndex(e => e.MemberServiceId, "IX_SeparationInvolvedMembers_MemberServiceId");

            entity.HasIndex(e => e.SeparationRequestId, "IX_SeparationInvolvedMembers_SeparationRequestId");

            entity.HasIndex(e => e.StatusId, "IX_SeparationInvolvedMembers_StatusId");

            entity.HasOne(d => d.MemberService).WithMany(p => p.SeparationInvolvedMembers)
                .HasForeignKey(d => d.MemberServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.SeparationRequest).WithMany(p => p.SeparationInvolvedMembers).HasForeignKey(d => d.SeparationRequestId);

            entity.HasOne(d => d.Status).WithMany(p => p.SeparationInvolvedMembers).HasForeignKey(d => d.StatusId);
        });

        modelBuilder.Entity<SeparationPlanRequest>(entity =>
        {
            entity.ToTable("SeparationPlanRequests", "cedo");

            entity.HasIndex(e => e.CityId, "IX_SeparationPlanRequests_CityId");

            entity.HasIndex(e => e.MunicipalityId, "IX_SeparationPlanRequests_MunicipalityId");

            entity.HasIndex(e => e.ScopeRequestTypeId, "IX_SeparationPlanRequests_ScopeRequestTypeId");

            entity.HasIndex(e => e.ServiceRequestId, "IX_SeparationPlanRequests_ServiceRequestId");

            entity.HasIndex(e => e.StatusId, "IX_SeparationPlanRequests_StatusId");

            entity.Property(e => e.Address).HasMaxLength(2048);
            entity.Property(e => e.LicenseNumber).HasMaxLength(128);
            entity.Property(e => e.ReqPlaque).HasMaxLength(128);
            entity.Property(e => e.ReqRegion).HasMaxLength(128);
            entity.Property(e => e.ReqUnit).HasMaxLength(128);
            entity.Property(e => e.ScopeRequestTypeId).HasDefaultValue(1);
            entity.Property(e => e.WorkEndNumber).HasMaxLength(128);

            entity.HasOne(d => d.City).WithMany(p => p.SeparationPlanRequests)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Municipality).WithMany(p => p.SeparationPlanRequests).HasForeignKey(d => d.MunicipalityId);

            entity.HasOne(d => d.ScopeRequestType).WithMany(p => p.SeparationPlanRequests).HasForeignKey(d => d.ScopeRequestTypeId);

            entity.HasOne(d => d.ServiceRequest).WithMany(p => p.SeparationPlanRequests).HasForeignKey(d => d.ServiceRequestId);

            entity.HasOne(d => d.Status).WithMany(p => p.SeparationPlanRequests).HasForeignKey(d => d.StatusId);
        });

        modelBuilder.Entity<SeparationPlanRequestStatus>(entity =>
        {
            entity.ToTable("SeparationPlanRequestStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<SeparationPlanRequestType>(entity =>
        {
            entity.ToTable("SeparationPlanRequestTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<SeparationRequestSurveie>(entity =>
        {
            entity.ToTable("SeparationRequestSurveies", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_SeparationRequestSurveies_InvolvedMemberId");

            entity.HasIndex(e => e.SeparationRequestId, "IX_SeparationRequestSurveies_SeparationRequestId");

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.SeparationRequestSurveies)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.SeparationRequest).WithMany(p => p.SeparationRequestSurveies).HasForeignKey(d => d.SeparationRequestId);
        });

        modelBuilder.Entity<ServiceField>(entity =>
        {
            entity.ToTable("ServiceFields", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ServiceRequest>(entity =>
        {
            entity.ToTable("ServiceRequests", "cedo");

            entity.HasIndex(e => e.RequestStatusId, "IX_ServiceRequests_RequestStatusId");

            entity.HasIndex(e => e.RequestTypeId, "IX_ServiceRequests_RequestTypeId");

            entity.HasIndex(e => e.UserId, "IX_ServiceRequests_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.RequestStatus).WithMany(p => p.ServiceRequests).HasForeignKey(d => d.RequestStatusId);

            entity.HasOne(d => d.RequestType).WithMany(p => p.ServiceRequests).HasForeignKey(d => d.RequestTypeId);

            entity.HasOne(d => d.User).WithMany(p => p.ServiceRequests).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<ServiceType>(entity =>
        {
            entity.ToTable("ServiceTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.ToTable("Settings", "puyahlpdsk");
        });

        modelBuilder.Entity<Sgaletter>(entity =>
        {
            entity.ToTable("SGALetters", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_SGALetters_ConstructionLicenseId");

            entity.HasIndex(e => e.LetterTypeId, "IX_SGALetters_LetterTypeId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.RegNumber).HasMaxLength(128);
            entity.Property(e => e.Subject).HasMaxLength(512);

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.Sgaletters).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.LetterType).WithMany(p => p.Sgaletters).HasForeignKey(d => d.LetterTypeId);
        });

        modelBuilder.Entity<SgaletterType>(entity =>
        {
            entity.ToTable("SGALetterTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<Signatory>(entity =>
        {
            entity.ToTable("Signatories", "cedo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Position)
                .HasMaxLength(256)
                .HasDefaultValue("");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Signatory)
                .HasForeignKey<Signatory>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SmsAccount>(entity =>
        {
            entity.Property(e => e.Number).HasMaxLength(64);
            entity.Property(e => e.Password).HasMaxLength(64);
            entity.Property(e => e.Provider).HasMaxLength(512);
            entity.Property(e => e.Username).HasMaxLength(64);
        });

        modelBuilder.Entity<SolutionResource>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.SolutionResource).HasForeignKey<SolutionResource>(d => d.Id);
        });

        modelBuilder.Entity<SqlServerDataSourceConfig>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Database).HasMaxLength(128);
            entity.Property(e => e.Password).HasMaxLength(128);
            entity.Property(e => e.Server).HasMaxLength(128);
            entity.Property(e => e.UserName).HasMaxLength(128);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.SqlServerDataSourceConfig).HasForeignKey<SqlServerDataSourceConfig>(d => d.Id);
        });

        modelBuilder.Entity<StepOwnerPool>(entity =>
        {
            entity.HasIndex(e => e.StepId, "IX_StepOwnerPools_StepId");

            entity.HasIndex(e => e.UserId, "IX_StepOwnerPools_UserId");

            entity.HasOne(d => d.Step).WithMany(p => p.StepOwnerPools).HasForeignKey(d => d.StepId);

            entity.HasOne(d => d.User).WithMany(p => p.StepOwnerPools)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<StepPending>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("StepPendings");

            entity.Property(e => e.FirstName).HasMaxLength(128);
            entity.Property(e => e.LastName).HasMaxLength(128);
        });

        modelBuilder.Entity<StepRecordDataLog>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.StepRecordDataLog).HasForeignKey<StepRecordDataLog>(d => d.Id);
        });

        modelBuilder.Entity<StepRecordLog>(entity =>
        {
            entity.HasIndex(e => new { e.RecordId, e.Entity }, "StepRecordLogs_IDX1");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Entity).HasMaxLength(256);
            entity.Property(e => e.RecordId).HasMaxLength(256);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.StepRecordLog).HasForeignKey<StepRecordLog>(d => d.Id);
        });

        modelBuilder.Entity<StructureExecutiveOperationsForm>(entity =>
        {
            entity.ToTable("StructureExecutiveOperationsForms", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_StructureExecutiveOperationsForms_InvolvedMemberId");

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.StructureExecutiveOperationsForms)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<StructurePlanControlForm>(entity =>
        {
            entity.ToTable("StructurePlanControlForms", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_StructurePlanControlForms_InvolvedMemberId");

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.StructurePlanControlForms)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<StructureRedLinesForm>(entity =>
        {
            entity.ToTable("StructureRedLinesForms", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_StructureRedLinesForms_InvolvedMemberId");

            entity.Property(e => e.Description).HasMaxLength(1024);

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.StructureRedLinesForms)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<StructureSafetyForm>(entity =>
        {
            entity.ToTable("StructureSafetyForms", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_StructureSafetyForms_ConstructionLicenseId");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_StructureSafetyForms_InvolvedMemberId");

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.StructureSafetyForms).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.StructureSafetyForms).HasForeignKey(d => d.InvolvedMemberId);
        });

        modelBuilder.Entity<StructureSpecForm>(entity =>
        {
            entity.ToTable("StructureSpecForms", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_StructureSpecForms_InvolvedMemberId");

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.StructureSpecForms)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<StructureType>(entity =>
        {
            entity.ToTable("StructureTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<StudyGrade>(entity =>
        {
            entity.ToTable("StudyGrades", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<StudyRecord>(entity =>
        {
            entity.ToTable("StudyRecords", "cedo");

            entity.HasIndex(e => e.MemberId, "IX_StudyRecords_MemberId");

            entity.HasIndex(e => e.StudyGradeId, "IX_StudyRecords_StudyGradeId");

            entity.HasIndex(e => e.UniversityId, "IX_StudyRecords_UniversityId");

            entity.HasOne(d => d.Member).WithMany(p => p.StudyRecords).HasForeignKey(d => d.MemberId);

            entity.HasOne(d => d.StudyGrade).WithMany(p => p.StudyRecords).HasForeignKey(d => d.StudyGradeId);

            entity.HasOne(d => d.University).WithMany(p => p.StudyRecords).HasForeignKey(d => d.UniversityId);
        });

        modelBuilder.Entity<SubWorkItemSetting>(entity =>
        {
            entity.ToTable("SubWorkItemSettings", "cedo");

            entity.HasIndex(e => e.CityId, "IX_SubWorkItemSettings_CityId");

            entity.HasIndex(e => e.DossierTypeId, "IX_SubWorkItemSettings_DossierTypeId");

            entity.HasIndex(e => e.ServiceTypeId, "IX_SubWorkItemSettings_ServiceTypeId");

            entity.HasIndex(e => e.SubGroupId, "IX_SubWorkItemSettings_SubGroupId");

            entity.HasOne(d => d.City).WithMany(p => p.SubWorkItemSettings).HasForeignKey(d => d.CityId);

            entity.HasOne(d => d.DossierType).WithMany(p => p.SubWorkItemSettings).HasForeignKey(d => d.DossierTypeId);

            entity.HasOne(d => d.ServiceType).WithMany(p => p.SubWorkItemSettings).HasForeignKey(d => d.ServiceTypeId);

            entity.HasOne(d => d.SubGroup).WithMany(p => p.SubWorkItemSettings)
                .HasForeignKey(d => d.SubGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SupervisionCandidate>(entity =>
        {
            entity.ToTable("SupervisionCandidates", "cedo");

            entity.HasIndex(e => e.MemberId, "IX_SupervisionCandidates_MemberId");

            entity.HasIndex(e => e.StatusId, "IX_SupervisionCandidates_StatusId");

            entity.HasIndex(e => e.SupervisionFieldId, "IX_SupervisionCandidates_SupervisionFieldId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(1024);

            entity.HasOne(d => d.Member).WithMany(p => p.SupervisionCandidates)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Status).WithMany(p => p.SupervisionCandidates).HasForeignKey(d => d.StatusId);

            entity.HasOne(d => d.SupervisionField).WithMany(p => p.SupervisionCandidates).HasForeignKey(d => d.SupervisionFieldId);
        });

        modelBuilder.Entity<SupervisionCandidateStatus>(entity =>
        {
            entity.ToTable("SupervisionCandidateStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<SupervisionField>(entity =>
        {
            entity.ToTable("SupervisionFields", "cedo");

            entity.HasIndex(e => e.FieldId, "IX_SupervisionFields_FieldId");

            entity.HasIndex(e => e.MinGradeId, "IX_SupervisionFields_MinGradeId");

            entity.HasIndex(e => e.SupervisionOrderId, "IX_SupervisionFields_SupervisionOrderId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Field).WithMany(p => p.SupervisionFields)
                .HasForeignKey(d => d.FieldId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.MinGrade).WithMany(p => p.SupervisionFields)
                .HasForeignKey(d => d.MinGradeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.SupervisionOrder).WithMany(p => p.SupervisionFields).HasForeignKey(d => d.SupervisionOrderId);
        });

        modelBuilder.Entity<SupervisionItemAnswer>(entity =>
        {
            entity.ToTable("SupervisionItemAnswers", "cedo");

            entity.HasIndex(e => e.FormId, "IX_SupervisionItemAnswers_FormId");

            entity.HasIndex(e => e.ItemId, "IX_SupervisionItemAnswers_ItemId");

            entity.Property(e => e.Answer).HasMaxLength(128);

            entity.HasOne(d => d.Form).WithMany(p => p.SupervisionItemAnswers).HasForeignKey(d => d.FormId);

            entity.HasOne(d => d.Item).WithMany(p => p.SupervisionItemAnswers).HasForeignKey(d => d.ItemId);
        });

        modelBuilder.Entity<SupervisionOrder>(entity =>
        {
            entity.ToTable("SupervisionOrders", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_SupervisionOrders_ConstructionLicenseId");

            entity.HasIndex(e => e.StatusId, "IX_SupervisionOrders_StatusId");

            entity.HasIndex(e => e.TypeId, "IX_SupervisionOrders_TypeId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.SupervisionOrders).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.Status).WithMany(p => p.SupervisionOrders).HasForeignKey(d => d.StatusId);

            entity.HasOne(d => d.Type).WithMany(p => p.SupervisionOrders).HasForeignKey(d => d.TypeId);
        });

        modelBuilder.Entity<SupervisionOrderStatus>(entity =>
        {
            entity.ToTable("SupervisionOrderStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<SupervisionOrderType>(entity =>
        {
            entity.ToTable("SupervisionOrderTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<SupervisionSetting>(entity =>
        {
            entity.ToTable("SupervisionSettings", "cedo");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<SupervisionStep>(entity =>
        {
            entity.ToTable("SupervisionSteps", "cedo");

            entity.HasIndex(e => e.FieldId, "IX_SupervisionSteps_FieldId");

            entity.Property(e => e.Title).HasMaxLength(512);

            entity.HasOne(d => d.Field).WithMany(p => p.SupervisionSteps).HasForeignKey(d => d.FieldId);
        });

        modelBuilder.Entity<SupervisionStepForm>(entity =>
        {
            entity.ToTable("SupervisionStepForms", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_SupervisionStepForms_InvolvedMemberId");

            entity.HasIndex(e => e.StepId, "IX_SupervisionStepForms_StepId");

            entity.HasIndex(e => e.StepId, "IX_SupervisionStepForms_StepId_C7457");

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.SupervisionStepForms)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Step).WithMany(p => p.SupervisionStepForms)
                .HasForeignKey(d => d.StepId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SupervisionStepItem>(entity =>
        {
            entity.ToTable("SupervisionStepItems", "cedo");

            entity.HasIndex(e => e.GroupId, "IX_SupervisionStepItems_GroupId");

            entity.Property(e => e.Title).HasMaxLength(512);

            entity.HasOne(d => d.Group).WithMany(p => p.SupervisionStepItems).HasForeignKey(d => d.GroupId);
        });

        modelBuilder.Entity<SupervisionStepItemGroup>(entity =>
        {
            entity.ToTable("SupervisionStepItemGroups", "cedo");

            entity.HasIndex(e => e.ServiceTypeId, "IX_SupervisionStepItemGroups_ServiceTypeId");

            entity.HasIndex(e => e.SupervisionStepId, "IX_SupervisionStepItemGroups_SupervisionStepId");

            entity.Property(e => e.Title).HasMaxLength(512);

            entity.HasOne(d => d.ServiceType).WithMany(p => p.SupervisionStepItemGroups).HasForeignKey(d => d.ServiceTypeId);

            entity.HasOne(d => d.SupervisionStep).WithMany(p => p.SupervisionStepItemGroups).HasForeignKey(d => d.SupervisionStepId);
        });

        modelBuilder.Entity<SupervisionStepItemOption>(entity =>
        {
            entity.ToTable("SupervisionStepItemOptions", "cedo");

            entity.HasIndex(e => e.SupervisionStepItemId, "IX_SupervisionStepItemOptions_SupervisionStepItemId");

            entity.Property(e => e.ItemOption).HasMaxLength(128);

            entity.HasOne(d => d.SupervisionStepItem).WithMany(p => p.SupervisionStepItemOptions).HasForeignKey(d => d.SupervisionStepItemId);
        });

        modelBuilder.Entity<SupervisionStepPayment>(entity =>
        {
            entity.ToTable("SupervisionStepPayments", "cedo");

            entity.HasIndex(e => e.InvolvedMemberId, "IX_SupervisionStepPayments_InvolvedMemberId");

            entity.HasIndex(e => e.PaymentStatusId, "IX_SupervisionStepPayments_PaymentStatusId");

            entity.HasIndex(e => e.SupervisionStepId, "IX_SupervisionStepPayments_SupervisionStepId");

            entity.Property(e => e.PaymentStatusId).HasDefaultValue(1);

            entity.HasOne(d => d.InvolvedMember).WithMany(p => p.SupervisionStepPayments)
                .HasForeignKey(d => d.InvolvedMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PaymentStatus).WithMany(p => p.SupervisionStepPayments).HasForeignKey(d => d.PaymentStatusId);

            entity.HasOne(d => d.SupervisionStep).WithMany(p => p.SupervisionStepPayments).HasForeignKey(d => d.SupervisionStepId);
        });

        modelBuilder.Entity<SupervisionStepPaymentStatus>(entity =>
        {
            entity.ToTable("SupervisionStepPaymentStatuses", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<SupportTicket>(entity =>
        {
            entity.ToTable("SupportTickets", "puyahlpdsk");

            entity.HasIndex(e => e.DepartmentId, "IX_SupportTickets_DepartmentId");

            entity.HasIndex(e => e.StatusId, "IX_SupportTickets_StatusId");

            entity.Property(e => e.Subject).HasMaxLength(512);
            entity.Property(e => e.TrackCode).HasMaxLength(128);

            entity.HasOne(d => d.Department).WithMany(p => p.SupportTickets).HasForeignKey(d => d.DepartmentId);

            entity.HasOne(d => d.Status).WithMany(p => p.SupportTickets).HasForeignKey(d => d.StatusId);
        });

        modelBuilder.Entity<SupportedCulture>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(5);
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.Property(e => e.ApiSecretKey)
                .HasMaxLength(128)
                .HasDefaultValue("PUYA.BPMS.Api.SecretCode");
            entity.Property(e => e.ErrorLogsMaxAge).HasDefaultValue(14);
            entity.Property(e => e.VisitStatsMaxAge).HasDefaultValue(730);
            entity.Property(e => e.WebServiceLogsMaxAge).HasDefaultValue(30);
        });

        modelBuilder.Entity<TakenUserAction>(entity =>
        {
            entity.HasIndex(e => e.ActionId, "IX_TakenUserActions_ActionId");

            entity.HasIndex(e => e.StepId, "IX_TakenUserActions_StepId");

            entity.HasOne(d => d.Action).WithMany(p => p.TakenUserActions).HasForeignKey(d => d.ActionId);

            entity.HasOne(d => d.Step).WithMany(p => p.TakenUserActions)
                .HasForeignKey(d => d.StepId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TaskType>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(512);
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<TeacherLesson>(entity =>
        {
            entity.ToTable("TeacherLessons", "cedo");

            entity.HasIndex(e => e.LessonId, "IX_TeacherLessons_LessonId");

            entity.HasIndex(e => e.MemberServiceId, "IX_TeacherLessons_MemberServiceId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Lesson).WithMany(p => p.TeacherLessons).HasForeignKey(d => d.LessonId);

            entity.HasOne(d => d.MemberService).WithMany(p => p.TeacherLessons)
                .HasForeignKey(d => d.MemberServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Temp>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Temp");

            entity.Property(e => e.A).HasMaxLength(50);
            entity.Property(e => e.B).HasMaxLength(50);
            entity.Property(e => e.C).HasMaxLength(50);
        });

        modelBuilder.Entity<TempInvolvedMember>(entity =>
        {
            entity.ToTable("TempInvolvedMember", "cedo");
        });

        modelBuilder.Entity<Territory>(entity =>
        {
            entity.ToTable("Territories", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_Territories_ConstructionLicenseId");

            entity.HasIndex(e => e.PasswayTypeId, "IX_Territories_PasswayTypeId");

            entity.HasIndex(e => e.TerritoryTypeId, "IX_Territories_TerritoryTypeId");

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.Territories).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.PasswayType).WithMany(p => p.Territories).HasForeignKey(d => d.PasswayTypeId);

            entity.HasOne(d => d.TerritoryType).WithMany(p => p.Territories).HasForeignKey(d => d.TerritoryTypeId);
        });

        modelBuilder.Entity<TerritoryType>(entity =>
        {
            entity.ToTable("TerritoryTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<TicketReply>(entity =>
        {
            entity.ToTable("TicketReplies", "puyahlpdsk");

            entity.HasIndex(e => e.TicketId, "IX_TicketReplies_TicketId");

            entity.HasIndex(e => e.UserId, "IX_TicketReplies_UserId");

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketReplies).HasForeignKey(d => d.TicketId);

            entity.HasOne(d => d.User).WithMany(p => p.TicketReplies).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<TicketStatus>(entity =>
        {
            entity.ToTable("TicketStatuses", "puyahlpdsk");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<TimerEvent>(entity =>
        {
            entity.HasIndex(e => e.TimerTypeId, "IX_TimerEvents_TimerTypeId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.TimerEvent).HasForeignKey<TimerEvent>(d => d.Id);

            entity.HasOne(d => d.TimerType).WithMany(p => p.TimerEvents).HasForeignKey(d => d.TimerTypeId);
        });

        modelBuilder.Entity<TimerTick>(entity =>
        {
            entity.HasIndex(e => e.InvokeAt, "IX_TimerTicks_InvokeAt_46915");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.TimerTick).HasForeignKey<TimerTick>(d => d.Id);
        });

        modelBuilder.Entity<TimerType>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<TrackCode>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("TrackCode");

            entity.Property(e => e.Entity).HasMaxLength(256);
            entity.Property(e => e.FirstName).HasMaxLength(128);
            entity.Property(e => e.LastName).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.RecordId).HasMaxLength(256);
            entity.Property(e => e.TrackCode1).HasColumnName("TrackCode");
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<TwoFactorMode>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<University>(entity =>
        {
            entity.ToTable("Universities", "cedo");

            entity.Property(e => e.Title).HasMaxLength(256);
        });

        modelBuilder.Entity<UsageType>(entity =>
        {
            entity.ToTable("UsageTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<UserActionTask>(entity =>
        {
            entity.HasIndex(e => e.ViewId, "IX_UserActionTasks_ViewId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ViewUrl).HasMaxLength(2048);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.UserActionTask).HasForeignKey<UserActionTask>(d => d.Id);

            entity.HasOne(d => d.View).WithMany(p => p.UserActionTasks).HasForeignKey(d => d.ViewId);
        });

        modelBuilder.Entity<UserActionTaskAction>(entity =>
        {
            entity.HasIndex(e => e.UniqueId, "IX_UserActionTaskActions_UniqueId_4FB52");

            entity.HasIndex(e => e.UserActionTaskId, "IX_UserActionTaskActions_UserActionTaskId");

            entity.HasIndex(e => e.UserActionTaskId, "IX_UserActionTaskActions_UserActionTaskId_F93E6");

            entity.Property(e => e.Title)
                .HasMaxLength(256)
                .HasDefaultValue("");

            entity.HasOne(d => d.UserActionTask).WithMany(p => p.UserActionTaskActions).HasForeignKey(d => d.UserActionTaskId);
        });

        modelBuilder.Entity<UserDelegation>(entity =>
        {
            entity.ToTable("UserDelegations", "cedo");

            entity.HasIndex(e => e.DelegateRecieverId, "IX_UserDelegations_DelegateRecieverId");

            entity.HasIndex(e => e.DelegateSenderId, "IX_UserDelegations_DelegateSenderId");

            entity.HasOne(d => d.DelegateReciever).WithMany(p => p.UserDelegationDelegateRecievers)
                .HasForeignKey(d => d.DelegateRecieverId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.DelegateSender).WithMany(p => p.UserDelegationDelegateSenders)
                .HasForeignKey(d => d.DelegateSenderId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasIndex(e => e.NationalCode, "IX_UserProfiles_NationalCode_1A39F");

            entity.HasIndex(e => e.NationalCode, "IX_UserProfiles_NationalCode_F3140");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BirthCertId).HasMaxLength(20);
            entity.Property(e => e.Comment).HasMaxLength(1024);
            entity.Property(e => e.Father).HasMaxLength(128);
            entity.Property(e => e.FirstName).HasMaxLength(128);
            entity.Property(e => e.LastName).HasMaxLength(128);
            entity.Property(e => e.NationalCode).HasMaxLength(10);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.UserProfile).HasForeignKey<UserProfile>(d => d.Id);
        });

        modelBuilder.Entity<UserTypePhoneNumber>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("UserTypePhoneNumbers");

            entity.Property(e => e.FirstName).HasMaxLength(128);
            entity.Property(e => e.LastName).HasMaxLength(128);
            entity.Property(e => e.ServiceField).HasMaxLength(128);
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<ValidMemberService>(entity =>
        {
            entity.ToTable("ValidMemberServices", "cedo");

            entity.HasIndex(e => e.LegalMemberTypeId, "IX_ValidMemberServices_LegalMemberTypeId");

            entity.HasIndex(e => e.MemberFieldId, "IX_ValidMemberServices_MemberFieldId");

            entity.HasIndex(e => e.MemberTypeId, "IX_ValidMemberServices_MemberTypeId");

            entity.HasIndex(e => e.MembershipTypeId, "IX_ValidMemberServices_MembershipTypeId");

            entity.HasIndex(e => e.ServiceFieldId, "IX_ValidMemberServices_ServiceFieldId");

            entity.HasIndex(e => e.ServiceTypeId, "IX_ValidMemberServices_ServiceTypeId");

            entity.HasOne(d => d.LegalMemberType).WithMany(p => p.ValidMemberServices).HasForeignKey(d => d.LegalMemberTypeId);

            entity.HasOne(d => d.MemberField).WithMany(p => p.ValidMemberServiceMemberFields)
                .HasForeignKey(d => d.MemberFieldId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.MemberType).WithMany(p => p.ValidMemberServices)
                .HasForeignKey(d => d.MemberTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.MembershipType).WithMany(p => p.ValidMemberServices)
                .HasForeignKey(d => d.MembershipTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ServiceField).WithMany(p => p.ValidMemberServiceServiceFields).HasForeignKey(d => d.ServiceFieldId);

            entity.HasOne(d => d.ServiceType).WithMany(p => p.ValidMemberServices)
                .HasForeignKey(d => d.ServiceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ViewAction>(entity =>
        {
            entity.HasIndex(e => e.ActionId, "IX_ViewActions_ActionId");

            entity.HasIndex(e => e.ViewId, "IX_ViewActions_ViewId");

            entity.HasOne(d => d.Action).WithMany(p => p.ViewActions).HasForeignKey(d => d.ActionId);

            entity.HasOne(d => d.View).WithMany(p => p.ViewActions)
                .HasForeignKey(d => d.ViewId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ViewComponent>(entity =>
        {
            entity.HasIndex(e => e.ProjectId, "IX_ViewComponents_ProjectId");

            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.Project).WithMany(p => p.ViewComponents).HasForeignKey(d => d.ProjectId);
        });

        modelBuilder.Entity<ViewComponentView>(entity =>
        {
            entity.HasIndex(e => e.ViewComponentId, "IX_ViewComponentViews_ViewComponentId");

            entity.HasIndex(e => e.ViewId, "IX_ViewComponentViews_ViewId");

            entity.HasOne(d => d.ViewComponent).WithMany(p => p.ViewComponentViews).HasForeignKey(d => d.ViewComponentId);

            entity.HasOne(d => d.View).WithMany(p => p.ViewComponentViews)
                .HasForeignKey(d => d.ViewId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ViewType>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<ViolationReport>(entity =>
        {
            entity.ToTable("ViolationReports", "cedo");

            entity.HasIndex(e => e.ConstructionLicenseId, "IX_ViolationReports_ConstructionLicenseId");

            entity.HasIndex(e => e.ReporterMemberId, "IX_ViolationReports_ReporterMemberId");

            entity.HasOne(d => d.ConstructionLicense).WithMany(p => p.ViolationReports).HasForeignKey(d => d.ConstructionLicenseId);

            entity.HasOne(d => d.ReporterMember).WithMany(p => p.ViolationReports)
                .HasForeignKey(d => d.ReporterMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ViolationReportDetail>(entity =>
        {
            entity.ToTable("ViolationReportDetails", "cedo");

            entity.HasIndex(e => e.MainSubjectId, "IX_ViolationReportDetails_MainSubjectId");

            entity.HasIndex(e => e.ViolationReportId, "IX_ViolationReportDetails_ViolationReportId");

            entity.Property(e => e.Detail).HasMaxLength(2048);
            entity.Property(e => e.Subject).HasMaxLength(256);

            entity.HasOne(d => d.MainSubject).WithMany(p => p.ViolationReportDetails).HasForeignKey(d => d.MainSubjectId);

            entity.HasOne(d => d.ViolationReport).WithMany(p => p.ViolationReportDetails).HasForeignKey(d => d.ViolationReportId);
        });

        modelBuilder.Entity<ViolationReportSubjectType>(entity =>
        {
            entity.ToTable("ViolationReportSubjectTypes", "cedo");

            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<ViolationReportUser>(entity =>
        {
            entity.ToTable("ViolationReportUsers", "cedo");

            entity.HasIndex(e => e.CityId, "IX_ViolationReportUsers_CityId");

            entity.HasIndex(e => e.UserId, "IX_ViolationReportUsers_UserId");

            entity.HasOne(d => d.City).WithMany(p => p.ViolationReportUsers)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.ViolationReportUsers).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<VwBillDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwBillDetail");

            entity.Property(e => e.DossierNumber).HasMaxLength(64);
            entity.Property(e => e.Expr2).HasMaxLength(128);
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<VwBuildingGroupSetting>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwBuildingGroupSetting");

            entity.Property(e => e.Expr2).HasMaxLength(128);
            entity.Property(e => e.Expr3).HasMaxLength(128);
            entity.Property(e => e.Expr4).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);
        });

        modelBuilder.Entity<VwCapacity>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwCapacity");

            entity.Property(e => e.Status).HasMaxLength(128);
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<VwCapacityCalculationsB200>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwCapacityCalculationsB200");

            entity.Property(e => e.Expr1).HasMaxLength(128);
            entity.Property(e => e.Expr2).HasMaxLength(128);
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<VwCapacityDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwCapacityDetail");

            entity.Property(e => e.FirstName).HasMaxLength(128);
            entity.Property(e => e.LastName).HasMaxLength(128);
        });

        modelBuilder.Entity<VwConstructionLisenceLab>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwConstructionLisenceLab");

            entity.Property(e => e.DossierNumber).HasMaxLength(64);
            entity.Property(e => e.Expr1).HasMaxLength(128);
            entity.Property(e => e.Expr2).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<VwContractDossier>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwContractDossiers");

            entity.Property(e => e.Code).HasMaxLength(128);
            entity.Property(e => e.DossierNumber).HasMaxLength(64);
            entity.Property(e => e.DossierSerial).HasMaxLength(50);
            entity.Property(e => e.Expr1).HasMaxLength(128);
            entity.Property(e => e.Expr2).HasMaxLength(128);
            entity.Property(e => e.Expr3).HasMaxLength(128);
            entity.Property(e => e.LicenseNumber).HasMaxLength(64);
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<VwHistoryCapacity>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwHistoryCapacity");

            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<VwIncompleteBill>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwIncompleteBills");

            entity.Property(e => e.DossierNumber).HasMaxLength(64);
            entity.Property(e => e.Name).HasMaxLength(128);
        });

        modelBuilder.Entity<VwInvolvedMember>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwInvolvedMembers");

            entity.Property(e => e.DossierNumber).HasMaxLength(64);
            entity.Property(e => e.Expr2).HasMaxLength(128);
            entity.Property(e => e.Expr3).HasMaxLength(128);
            entity.Property(e => e.FirstName).HasMaxLength(128);
            entity.Property(e => e.LastName).HasMaxLength(128);
            entity.Property(e => e.ServiceTypes).HasMaxLength(128);
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<VwLicenseInquery>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwLicenseInqueries");

            entity.Property(e => e.Code).HasMaxLength(128);
            entity.Property(e => e.LicenseNumber).HasMaxLength(128);
            entity.Property(e => e.NationalCode).HasMaxLength(10);
            entity.Property(e => e.OwnerFullName).HasMaxLength(128);
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<VwMemberCapacityDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwMemberCapacityDetails");

            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Field).HasMaxLength(128);
            entity.Property(e => e.Service).HasMaxLength(128);
        });

        modelBuilder.Entity<VwNeedExecuterDossier>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwNeedExecuterDossiers");

            entity.Property(e => e.DossierNumber).HasMaxLength(64);
            entity.Property(e => e.Name).HasMaxLength(128);
        });

        modelBuilder.Entity<VwNotReleased>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwNotReleased");

            entity.Property(e => e.DossierNumber).HasMaxLength(64);
            entity.Property(e => e.DossierSerial).HasMaxLength(50);
            entity.Property(e => e.Expr1).HasMaxLength(128);
            entity.Property(e => e.LicenseNumber).HasMaxLength(64);
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<VwOperator>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwOperator");

            entity.Property(e => e.Expr1).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<VwPaymentCallBackReport>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwPaymentCallBackReport");

            entity.Property(e => e.AccountNumber).HasMaxLength(128);
            entity.Property(e => e.BankName).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(2048);
            entity.Property(e => e.DossierSerial).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<VwPendingStep>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwPendingSteps");

            entity.Property(e => e.FirstName).HasMaxLength(128);
            entity.Property(e => e.LastName).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<VwPendingTimer>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwPendingTimers");

            entity.Property(e => e.Name).HasMaxLength(128);
        });

        modelBuilder.Entity<VwPlanControl>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwPlanControl");

            entity.Property(e => e.Description).HasMaxLength(1024);
            entity.Property(e => e.Expr1).HasMaxLength(128);
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<VwSeperationPlan>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwSeperationPlans");

            entity.Property(e => e.آخریننقشهبرداری).HasColumnName("آخرین نقشه برداری");
            entity.Property(e => e.آدرس).HasMaxLength(2048);
            entity.Property(e => e.اولیننقشهبرداری).HasColumnName("اولین نقشه برداری");
            entity.Property(e => e.تاریخثبت).HasColumnName("تاریخ ثبت");
            entity.Property(e => e.تاریخپروانه).HasColumnName("تاریخ پروانه");
            entity.Property(e => e.تعدادسقف).HasColumnName("تعداد سقف");
            entity.Property(e => e.تعدادسقفنقشهبردار).HasColumnName("تعداد سقف نقشه بردار");
            entity.Property(e => e.تعدادواحد).HasColumnName("تعداد واحد");
            entity.Property(e => e.شمارهعضویتمهندس)
                .HasMaxLength(32)
                .HasColumnName("شماره عضویت مهندس");
            entity.Property(e => e.شمارهپروانه)
                .HasMaxLength(128)
                .HasColumnName("شماره پروانه");
            entity.Property(e => e.شهرستان).HasMaxLength(128);
            entity.Property(e => e.متراژنقشهبردار).HasColumnName("متراژ نقشه بردار");
            entity.Property(e => e.موبایلمالک).HasColumnName("موبایل مالک");
            entity.Property(e => e.نامخانوادگی)
                .HasMaxLength(128)
                .HasColumnName("نام خانوادگی");
            entity.Property(e => e.نامخانوادگیمهندس)
                .HasMaxLength(128)
                .HasColumnName("نام خانوادگی مهندس");
            entity.Property(e => e.ناممالک)
                .HasMaxLength(128)
                .HasColumnName("نام مالک");
            entity.Property(e => e.ناممهندس)
                .HasMaxLength(128)
                .HasColumnName("نام مهندس");
            entity.Property(e => e.وضعیتمهندس)
                .HasMaxLength(128)
                .HasColumnName("وضعیت مهندس");
            entity.Property(e => e.وضعیتپرونده)
                .HasMaxLength(128)
                .HasColumnName("وضعیت پرونده");
            entity.Property(e => e.پلاکثبتی)
                .HasMaxLength(128)
                .HasColumnName("پلاک ثبتی");
            entity.Property(e => e.کدملی)
                .HasMaxLength(10)
                .HasColumnName("کد ملی");
        });

        modelBuilder.Entity<VwStepFom>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwStepFoms");

            entity.Property(e => e.DossierNumber).HasMaxLength(64);
            entity.Property(e => e.Expr1).HasMaxLength(128);
            entity.Property(e => e.Expr2).HasMaxLength(512);
            entity.Property(e => e.Expr3).HasMaxLength(512);
            entity.Property(e => e.Expr4).HasMaxLength(512);
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<VwSupervisorContractTempDossier>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwSupervisorContractTempDossier");

            entity.Property(e => e.Code).HasMaxLength(128);
            entity.Property(e => e.DossierNumber).HasMaxLength(64);
            entity.Property(e => e.DossierSerial).HasMaxLength(50);
            entity.Property(e => e.Expr1).HasMaxLength(128);
            entity.Property(e => e.Expr2).HasMaxLength(128);
            entity.Property(e => e.FirstName).HasMaxLength(128);
            entity.Property(e => e.LastName).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.NationalCode).HasMaxLength(10);
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<VwUserDeligation>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwUserDeligation");

            entity.Property(e => e.FirstName).HasMaxLength(128);
            entity.Property(e => e.LastName).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.Title).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(128);
        });

        modelBuilder.Entity<VwWorkItemsSync>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwWorkItemsSync");

            entity.Property(e => e.DossierNumber).HasMaxLength(64);
        });

        modelBuilder.Entity<WebPushSetting>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.WebPushSetting).HasForeignKey<WebPushSetting>(d => d.Id);
        });

        modelBuilder.Entity<WebPushSubscription>(entity =>
        {
            entity.HasIndex(e => e.WebPushSettingId, "IX_WebPushSubscriptions_WebPushSettingId");

            entity.HasOne(d => d.WebPushSetting).WithMany(p => p.WebPushSubscriptions).HasForeignKey(d => d.WebPushSettingId);
        });

        modelBuilder.Entity<WebService>(entity =>
        {
            entity.HasIndex(e => e.ProjectId, "IX_WebServices_ProjectId");

            entity.HasIndex(e => e.WebServiceTypeId, "IX_WebServices_WebServiceTypeId");

            entity.Property(e => e.ClientName).HasMaxLength(128);
            entity.Property(e => e.Url).HasMaxLength(1024);

            entity.HasOne(d => d.Project).WithMany(p => p.WebServices).HasForeignKey(d => d.ProjectId);

            entity.HasOne(d => d.WebServiceType).WithMany(p => p.WebServices).HasForeignKey(d => d.WebServiceTypeId);
        });

        modelBuilder.Entity<WebServiceMethod>(entity =>
        {
            entity.HasIndex(e => e.WebServiceId, "IX_WebServiceMethods_WebServiceId");

            entity.HasIndex(e => e.WebServiceMethodTypeId, "IX_WebServiceMethods_WebServiceMethodTypeId");

            entity.Property(e => e.BodyClrType).HasMaxLength(512);
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.Path).HasMaxLength(1024);
            entity.Property(e => e.ReturnClrType).HasMaxLength(512);

            entity.HasOne(d => d.WebService).WithMany(p => p.WebServiceMethods).HasForeignKey(d => d.WebServiceId);

            entity.HasOne(d => d.WebServiceMethodType).WithMany(p => p.WebServiceMethods).HasForeignKey(d => d.WebServiceMethodTypeId);
        });

        modelBuilder.Entity<WebServiceMethodRequestHeader>(entity =>
        {
            entity.HasIndex(e => e.MethodId, "IX_WebServiceMethodRequestHeaders_MethodId");

            entity.Property(e => e.Header).HasMaxLength(128);
            entity.Property(e => e.Value).HasMaxLength(1024);

            entity.HasOne(d => d.Method).WithMany(p => p.WebServiceMethodRequestHeaders).HasForeignKey(d => d.MethodId);
        });

        modelBuilder.Entity<WebServiceMethodRouteArgument>(entity =>
        {
            entity.HasIndex(e => e.MethodId, "IX_WebServiceMethodRouteArguments_MethodId");

            entity.Property(e => e.ClrDataType).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.Method).WithMany(p => p.WebServiceMethodRouteArguments).HasForeignKey(d => d.MethodId);
        });

        modelBuilder.Entity<WebServiceMethodType>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<WebServiceMethodUrlArgument>(entity =>
        {
            entity.HasIndex(e => e.MethodId, "IX_WebServiceMethodUrlArguments_MethodId");

            entity.Property(e => e.ClrDataType).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.Method).WithMany(p => p.WebServiceMethodUrlArguments).HasForeignKey(d => d.MethodId);
        });

        modelBuilder.Entity<WebServiceType>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<WorkItemSetting>(entity =>
        {
            entity.ToTable("WorkItemSettings", "cedo");

            entity.HasIndex(e => e.BuildingGroupId, "IX_WorkItemSettings_BuildingGroupId");

            entity.HasIndex(e => e.CityId, "IX_WorkItemSettings_CityId");

            entity.HasIndex(e => e.DossierTypeId, "IX_WorkItemSettings_DossierTypeId");

            entity.HasOne(d => d.BuildingGroup).WithMany(p => p.WorkItemSettings).HasForeignKey(d => d.BuildingGroupId);

            entity.HasOne(d => d.City).WithMany(p => p.WorkItemSettings).HasForeignKey(d => d.CityId);

            entity.HasOne(d => d.DossierType).WithMany(p => p.WorkItemSettings).HasForeignKey(d => d.DossierTypeId);
        });

        modelBuilder.Entity<Xpayment>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("xpayment");

            entity.Property(e => e.Am).HasColumnName("am");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Rem).HasColumnName("rem");
        });

        modelBuilder.Entity<Zzzzzzzzzz>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("zzzzzzzzzz");

            entity.Property(e => e.Index).HasColumnName("index");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
