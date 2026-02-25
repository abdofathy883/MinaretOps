using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSchema_SetDeleteBehaiourInTaskItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementLinks_Announcements_AnnouncementId",
                table: "AnnouncementLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_ArchivedTasks_AspNetUsers_EmployeeId",
                table: "ArchivedTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_ArchivedTasks_ClientServices_ClientServiceId",
                table: "ArchivedTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_ArchivedTasks_TaskGroups_TaskGroupId",
                table: "ArchivedTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_AspNetUsers_EmployeeId",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_BreakPeriods_AttendanceRecords_AttendanceRecordId",
                table: "BreakPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_AspNetUsers_AccountManagerId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientServiceCheckpoints_AspNetUsers_CompletedByEmployeeId",
                table: "ClientServiceCheckpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientServiceCheckpoints_ClientServices_ClientServiceId",
                table: "ClientServiceCheckpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientServiceCheckpoints_ServiceCheckpoints_ServiceCheckpointId",
                table: "ClientServiceCheckpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientServices_Clients_ClientId",
                table: "ClientServices");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientServices_Services_ServiceId",
                table: "ClientServices");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_AspNetUsers_EmployeeId",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Clients_ClientId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Currencies_CurrencyId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeOnBoardingInvitations_AspNetUsers_InvitedByUserId",
                table: "EmployeeOnBoardingInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_InternalTaskAssignments_AspNetUsers_UserId",
                table: "InternalTaskAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_InternalTaskAssignments_InternalTasks_InternalTaskId",
                table: "InternalTaskAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_JobDescriptions_AspNetRoles_RoleId",
                table: "JobDescriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_JobResponsibilities_JobDescriptions_JobDescriptionId",
                table: "JobResponsibilities");

            migrationBuilder.DropForeignKey(
                name: "FK_KPIIncedints_AspNetUsers_EmployeeId",
                table: "KPIIncedints");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadNotes_AspNetUsers_CreatedById",
                table: "LeadNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadNotes_SalesLeads_LeadId",
                table: "LeadNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadServices_SalesLeads_LeadId",
                table: "LeadServices");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadServices_Services_ServiceId",
                table: "LeadServices");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_EmployeeId",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_SalaryPayments_AspNetUsers_EmployeeId",
                table: "SalaryPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_SalaryPayments_SalaryPeriods_SalaryPeriodId",
                table: "SalaryPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_SalaryPeriods_AspNetUsers_EmployeeId",
                table: "SalaryPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesLeads_AspNetUsers_CreatedById",
                table: "SalesLeads");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesLeads_AspNetUsers_SalesRepId",
                table: "SalesLeads");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCheckpoints_Services_ServiceId",
                table: "ServiceCheckpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskComments_AspNetUsers_EmployeeId",
                table: "TaskComments");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskComments_Tasks_TaskId",
                table: "TaskComments");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCompletionResources_ArchivedTasks_ArchivedTaskId",
                table: "TaskCompletionResources");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCompletionResources_Tasks_TaskId",
                table: "TaskCompletionResources");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskGroups_ClientServices_ClientServiceId",
                table: "TaskGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistory_ArchivedTasks_ArchivedTaskId",
                table: "TaskHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistory_AspNetUsers_UpdatedById",
                table: "TaskHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistory_Tasks_TaskItemId",
                table: "TaskHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_EmployeeId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_ClientServices_ClientServiceId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskGroups_TaskGroupId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskHistory",
                table: "TaskHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskGroups",
                table: "TaskGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskComments",
                table: "TaskComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Services",
                table: "Services");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceCheckpoints",
                table: "ServiceCheckpoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SeoContents",
                table: "SeoContents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SalesLeads",
                table: "SalesLeads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SalaryPeriods",
                table: "SalaryPeriods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SalaryPayments",
                table: "SalaryPayments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveRequests",
                table: "LeaveRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeadNotes",
                table: "LeadNotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KPIIncedints",
                table: "KPIIncedints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobResponsibilities",
                table: "JobResponsibilities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobDescriptions",
                table: "JobDescriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InternalTasks",
                table: "InternalTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InternalTaskAssignments",
                table: "InternalTaskAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeOnBoardingInvitations",
                table: "EmployeeOnBoardingInvitations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contracts",
                table: "Contracts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactEntries",
                table: "ContactEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Complaints",
                table: "Complaints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientServices",
                table: "ClientServices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientServiceCheckpoints",
                table: "ClientServiceCheckpoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clients",
                table: "Clients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BreakPeriods",
                table: "BreakPeriods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttendanceRecords",
                table: "AttendanceRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ArchivedTasks",
                table: "ArchivedTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Announcements",
                table: "Announcements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnnouncementLinks",
                table: "AnnouncementLinks");

            migrationBuilder.EnsureSchema(
                name: "Communication");

            migrationBuilder.EnsureSchema(
                name: "Tasks");

            migrationBuilder.EnsureSchema(
                name: "HR");

            migrationBuilder.EnsureSchema(
                name: "Clients");

            migrationBuilder.EnsureSchema(
                name: "CRM");

            migrationBuilder.EnsureSchema(
                name: "Identity");

            migrationBuilder.EnsureSchema(
                name: "Content");

            migrationBuilder.EnsureSchema(
                name: "Services");

            migrationBuilder.RenameTable(
                name: "TaskCompletionResources",
                newName: "TaskCompletionResources",
                newSchema: "Tasks");

            migrationBuilder.RenameTable(
                name: "LeadServices",
                newName: "LeadServices",
                newSchema: "CRM");

            migrationBuilder.RenameTable(
                name: "Tasks",
                newName: "TaskItem",
                newSchema: "Tasks");

            migrationBuilder.RenameTable(
                name: "TaskHistory",
                newName: "TaskItemHistory",
                newSchema: "Tasks");

            migrationBuilder.RenameTable(
                name: "TaskGroups",
                newName: "TaskGroup",
                newSchema: "Tasks");

            migrationBuilder.RenameTable(
                name: "TaskComments",
                newName: "TaskComment",
                newSchema: "Tasks");

            migrationBuilder.RenameTable(
                name: "Services",
                newName: "Service",
                newSchema: "Services");

            migrationBuilder.RenameTable(
                name: "ServiceCheckpoints",
                newName: "ServiceCheckpoint",
                newSchema: "Services");

            migrationBuilder.RenameTable(
                name: "SeoContents",
                newName: "SeoContent",
                newSchema: "Content");

            migrationBuilder.RenameTable(
                name: "SalesLeads",
                newName: "SalesLead",
                newSchema: "CRM");

            migrationBuilder.RenameTable(
                name: "SalaryPeriods",
                newName: "SalaryPeriod",
                newSchema: "HR");

            migrationBuilder.RenameTable(
                name: "SalaryPayments",
                newName: "SalaryPayment",
                newSchema: "HR");

            migrationBuilder.RenameTable(
                name: "LeaveRequests",
                newName: "LeaveRequest",
                newSchema: "HR");

            migrationBuilder.RenameTable(
                name: "LeadNotes",
                newName: "LeadNote",
                newSchema: "CRM");

            migrationBuilder.RenameTable(
                name: "KPIIncedints",
                newName: "KPIIncedint",
                newSchema: "HR");

            migrationBuilder.RenameTable(
                name: "JobResponsibilities",
                newName: "JobResponsibility",
                newSchema: "HR");

            migrationBuilder.RenameTable(
                name: "JobDescriptions",
                newName: "JobDescription",
                newSchema: "HR");

            migrationBuilder.RenameTable(
                name: "InternalTasks",
                newName: "InternalTask",
                newSchema: "Tasks");

            migrationBuilder.RenameTable(
                name: "InternalTaskAssignments",
                newName: "InternalTaskAssignment",
                newSchema: "Tasks");

            migrationBuilder.RenameTable(
                name: "EmployeeOnBoardingInvitations",
                newName: "EmployeeOnBoardingInvitation",
                newSchema: "Identity");

            migrationBuilder.RenameTable(
                name: "Contracts",
                newName: "CustomContract",
                newSchema: "Clients");

            migrationBuilder.RenameTable(
                name: "ContactEntries",
                newName: "ContactEntry",
                newSchema: "CRM");

            migrationBuilder.RenameTable(
                name: "Complaints",
                newName: "Complaint",
                newSchema: "Communication");

            migrationBuilder.RenameTable(
                name: "ClientServices",
                newName: "ClientService",
                newSchema: "Clients");

            migrationBuilder.RenameTable(
                name: "ClientServiceCheckpoints",
                newName: "ClientServiceCheckpoint",
                newSchema: "Clients");

            migrationBuilder.RenameTable(
                name: "Clients",
                newName: "Client",
                newSchema: "Clients");

            migrationBuilder.RenameTable(
                name: "BreakPeriods",
                newName: "BreakPeriod",
                newSchema: "HR");

            migrationBuilder.RenameTable(
                name: "AttendanceRecords",
                newName: "AttendanceRecord",
                newSchema: "HR");

            migrationBuilder.RenameTable(
                name: "ArchivedTasks",
                newName: "ArchivedTask",
                newSchema: "Tasks");

            migrationBuilder.RenameTable(
                name: "Announcements",
                newName: "Announcement",
                newSchema: "Communication");

            migrationBuilder.RenameTable(
                name: "AnnouncementLinks",
                newName: "AnnouncementLink",
                newSchema: "Communication");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_TaskGroupId",
                schema: "Tasks",
                table: "TaskItem",
                newName: "IX_TaskItem_TaskGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_EmployeeId",
                schema: "Tasks",
                table: "TaskItem",
                newName: "IX_TaskItem_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_ClientServiceId",
                schema: "Tasks",
                table: "TaskItem",
                newName: "IX_TaskItem_ClientServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskHistory_UpdatedById",
                schema: "Tasks",
                table: "TaskItemHistory",
                newName: "IX_TaskItemHistory_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_TaskHistory_TaskItemId",
                schema: "Tasks",
                table: "TaskItemHistory",
                newName: "IX_TaskItemHistory_TaskItemId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskHistory_ArchivedTaskId",
                schema: "Tasks",
                table: "TaskItemHistory",
                newName: "IX_TaskItemHistory_ArchivedTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskGroups_ClientServiceId",
                schema: "Tasks",
                table: "TaskGroup",
                newName: "IX_TaskGroup_ClientServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskComments_TaskId",
                schema: "Tasks",
                table: "TaskComment",
                newName: "IX_TaskComment_TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskComments_EmployeeId",
                schema: "Tasks",
                table: "TaskComment",
                newName: "IX_TaskComment_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_Services_Title",
                schema: "Services",
                table: "Service",
                newName: "IX_Service_Title");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceCheckpoints_ServiceId_Order",
                schema: "Services",
                table: "ServiceCheckpoint",
                newName: "IX_ServiceCheckpoint_ServiceId_Order");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceCheckpoints_ServiceId",
                schema: "Services",
                table: "ServiceCheckpoint",
                newName: "IX_ServiceCheckpoint_ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_SeoContents_Route_Language",
                schema: "Content",
                table: "SeoContent",
                newName: "IX_SeoContent_Route_Language");

            migrationBuilder.RenameIndex(
                name: "IX_SalesLeads_SalesRepId",
                schema: "CRM",
                table: "SalesLead",
                newName: "IX_SalesLead_SalesRepId");

            migrationBuilder.RenameIndex(
                name: "IX_SalesLeads_CreatedById",
                schema: "CRM",
                table: "SalesLead",
                newName: "IX_SalesLead_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_SalesLeads_CreatedAt",
                schema: "CRM",
                table: "SalesLead",
                newName: "IX_SalesLead_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_SalaryPeriods_MonthLabel",
                schema: "HR",
                table: "SalaryPeriod",
                newName: "IX_SalaryPeriod_MonthLabel");

            migrationBuilder.RenameIndex(
                name: "IX_SalaryPeriods_EmployeeId_Year_Month",
                schema: "HR",
                table: "SalaryPeriod",
                newName: "IX_SalaryPeriod_EmployeeId_Year_Month");

            migrationBuilder.RenameIndex(
                name: "IX_SalaryPeriods_EmployeeId",
                schema: "HR",
                table: "SalaryPeriod",
                newName: "IX_SalaryPeriod_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_SalaryPayments_SalaryPeriodId",
                schema: "HR",
                table: "SalaryPayment",
                newName: "IX_SalaryPayment_SalaryPeriodId");

            migrationBuilder.RenameIndex(
                name: "IX_SalaryPayments_EmployeeId",
                schema: "HR",
                table: "SalaryPayment",
                newName: "IX_SalaryPayment_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_LeaveRequests_EmployeeId",
                schema: "HR",
                table: "LeaveRequest",
                newName: "IX_LeaveRequest_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_LeadNotes_LeadId",
                schema: "CRM",
                table: "LeadNote",
                newName: "IX_LeadNote_LeadId");

            migrationBuilder.RenameIndex(
                name: "IX_LeadNotes_CreatedById",
                schema: "CRM",
                table: "LeadNote",
                newName: "IX_LeadNote_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_KPIIncedints_EmployeeId",
                schema: "HR",
                table: "KPIIncedint",
                newName: "IX_KPIIncedint_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_JobResponsibilities_JobDescriptionId",
                schema: "HR",
                table: "JobResponsibility",
                newName: "IX_JobResponsibility_JobDescriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_JobDescriptions_RoleId",
                schema: "HR",
                table: "JobDescription",
                newName: "IX_JobDescription_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_InternalTaskAssignments_UserId",
                schema: "Tasks",
                table: "InternalTaskAssignment",
                newName: "IX_InternalTaskAssignment_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_InternalTaskAssignments_InternalTaskId",
                schema: "Tasks",
                table: "InternalTaskAssignment",
                newName: "IX_InternalTaskAssignment_InternalTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeOnBoardingInvitations_InvitedByUserId",
                schema: "Identity",
                table: "EmployeeOnBoardingInvitation",
                newName: "IX_EmployeeOnBoardingInvitation_InvitedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Contracts_CurrencyId",
                schema: "Clients",
                table: "CustomContract",
                newName: "IX_CustomContract_CurrencyId");

            migrationBuilder.RenameIndex(
                name: "IX_Contracts_ClientId",
                schema: "Clients",
                table: "CustomContract",
                newName: "IX_CustomContract_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_Complaints_EmployeeId",
                schema: "Communication",
                table: "Complaint",
                newName: "IX_Complaint_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientServices_ServiceId",
                schema: "Clients",
                table: "ClientService",
                newName: "IX_ClientService_ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientServices_ClientId",
                schema: "Clients",
                table: "ClientService",
                newName: "IX_ClientService_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientServiceCheckpoints_ServiceCheckpointId",
                schema: "Clients",
                table: "ClientServiceCheckpoint",
                newName: "IX_ClientServiceCheckpoint_ServiceCheckpointId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientServiceCheckpoints_CompletedByEmployeeId",
                schema: "Clients",
                table: "ClientServiceCheckpoint",
                newName: "IX_ClientServiceCheckpoint_CompletedByEmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientServiceCheckpoints_ClientServiceId_ServiceCheckpointId",
                schema: "Clients",
                table: "ClientServiceCheckpoint",
                newName: "IX_ClientServiceCheckpoint_ClientServiceId_ServiceCheckpointId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientServiceCheckpoints_ClientServiceId",
                schema: "Clients",
                table: "ClientServiceCheckpoint",
                newName: "IX_ClientServiceCheckpoint_ClientServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Clients_Name",
                schema: "Clients",
                table: "Client",
                newName: "IX_Client_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Clients_CompanyName",
                schema: "Clients",
                table: "Client",
                newName: "IX_Client_CompanyName");

            migrationBuilder.RenameIndex(
                name: "IX_Clients_AccountManagerId",
                schema: "Clients",
                table: "Client",
                newName: "IX_Client_AccountManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_BreakPeriods_AttendanceRecordId",
                schema: "HR",
                table: "BreakPeriod",
                newName: "IX_BreakPeriod_AttendanceRecordId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecords_EmployeeId_WorkDate",
                schema: "HR",
                table: "AttendanceRecord",
                newName: "IX_AttendanceRecord_EmployeeId_WorkDate");

            migrationBuilder.RenameIndex(
                name: "IX_ArchivedTasks_TaskGroupId",
                schema: "Tasks",
                table: "ArchivedTask",
                newName: "IX_ArchivedTask_TaskGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_ArchivedTasks_EmployeeId",
                schema: "Tasks",
                table: "ArchivedTask",
                newName: "IX_ArchivedTask_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_ArchivedTasks_ClientServiceId",
                schema: "Tasks",
                table: "ArchivedTask",
                newName: "IX_ArchivedTask_ClientServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Announcements_CreatedAt",
                schema: "Communication",
                table: "Announcement",
                newName: "IX_Announcement_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_AnnouncementLinks_AnnouncementId",
                schema: "Communication",
                table: "AnnouncementLink",
                newName: "IX_AnnouncementLink_AnnouncementId");

            migrationBuilder.AlterColumn<int>(
                name: "TaskGroupId",
                schema: "Tasks",
                table: "TaskItem",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TaskGroupId",
                schema: "Tasks",
                table: "ArchivedTask",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskItem",
                schema: "Tasks",
                table: "TaskItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskItemHistory",
                schema: "Tasks",
                table: "TaskItemHistory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskGroup",
                schema: "Tasks",
                table: "TaskGroup",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskComment",
                schema: "Tasks",
                table: "TaskComment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Service",
                schema: "Services",
                table: "Service",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceCheckpoint",
                schema: "Services",
                table: "ServiceCheckpoint",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SeoContent",
                schema: "Content",
                table: "SeoContent",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SalesLead",
                schema: "CRM",
                table: "SalesLead",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SalaryPeriod",
                schema: "HR",
                table: "SalaryPeriod",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SalaryPayment",
                schema: "HR",
                table: "SalaryPayment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveRequest",
                schema: "HR",
                table: "LeaveRequest",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeadNote",
                schema: "CRM",
                table: "LeadNote",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KPIIncedint",
                schema: "HR",
                table: "KPIIncedint",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobResponsibility",
                schema: "HR",
                table: "JobResponsibility",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobDescription",
                schema: "HR",
                table: "JobDescription",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InternalTask",
                schema: "Tasks",
                table: "InternalTask",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InternalTaskAssignment",
                schema: "Tasks",
                table: "InternalTaskAssignment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeOnBoardingInvitation",
                schema: "Identity",
                table: "EmployeeOnBoardingInvitation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomContract",
                schema: "Clients",
                table: "CustomContract",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactEntry",
                schema: "CRM",
                table: "ContactEntry",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Complaint",
                schema: "Communication",
                table: "Complaint",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientService",
                schema: "Clients",
                table: "ClientService",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientServiceCheckpoint",
                schema: "Clients",
                table: "ClientServiceCheckpoint",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Client",
                schema: "Clients",
                table: "Client",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BreakPeriod",
                schema: "HR",
                table: "BreakPeriod",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttendanceRecord",
                schema: "HR",
                table: "AttendanceRecord",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArchivedTask",
                schema: "Tasks",
                table: "ArchivedTask",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Announcement",
                schema: "Communication",
                table: "Announcement",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnnouncementLink",
                schema: "Communication",
                table: "AnnouncementLink",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementLink_Announcement_AnnouncementId",
                schema: "Communication",
                table: "AnnouncementLink",
                column: "AnnouncementId",
                principalSchema: "Communication",
                principalTable: "Announcement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArchivedTask_AspNetUsers_EmployeeId",
                schema: "Tasks",
                table: "ArchivedTask",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ArchivedTask_ClientService_ClientServiceId",
                schema: "Tasks",
                table: "ArchivedTask",
                column: "ClientServiceId",
                principalSchema: "Clients",
                principalTable: "ClientService",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ArchivedTask_TaskGroup_TaskGroupId",
                schema: "Tasks",
                table: "ArchivedTask",
                column: "TaskGroupId",
                principalSchema: "Tasks",
                principalTable: "TaskGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecord_AspNetUsers_EmployeeId",
                schema: "HR",
                table: "AttendanceRecord",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BreakPeriod_AttendanceRecord_AttendanceRecordId",
                schema: "HR",
                table: "BreakPeriod",
                column: "AttendanceRecordId",
                principalSchema: "HR",
                principalTable: "AttendanceRecord",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Client_AspNetUsers_AccountManagerId",
                schema: "Clients",
                table: "Client",
                column: "AccountManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientService_Client_ClientId",
                schema: "Clients",
                table: "ClientService",
                column: "ClientId",
                principalSchema: "Clients",
                principalTable: "Client",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientService_Service_ServiceId",
                schema: "Clients",
                table: "ClientService",
                column: "ServiceId",
                principalSchema: "Services",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientServiceCheckpoint_AspNetUsers_CompletedByEmployeeId",
                schema: "Clients",
                table: "ClientServiceCheckpoint",
                column: "CompletedByEmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientServiceCheckpoint_ClientService_ClientServiceId",
                schema: "Clients",
                table: "ClientServiceCheckpoint",
                column: "ClientServiceId",
                principalSchema: "Clients",
                principalTable: "ClientService",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientServiceCheckpoint_ServiceCheckpoint_ServiceCheckpointId",
                schema: "Clients",
                table: "ClientServiceCheckpoint",
                column: "ServiceCheckpointId",
                principalSchema: "Services",
                principalTable: "ServiceCheckpoint",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaint_AspNetUsers_EmployeeId",
                schema: "Communication",
                table: "Complaint",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomContract_Client_ClientId",
                schema: "Clients",
                table: "CustomContract",
                column: "ClientId",
                principalSchema: "Clients",
                principalTable: "Client",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomContract_Currencies_CurrencyId",
                schema: "Clients",
                table: "CustomContract",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeOnBoardingInvitation_AspNetUsers_InvitedByUserId",
                schema: "Identity",
                table: "EmployeeOnBoardingInvitation",
                column: "InvitedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InternalTaskAssignment_AspNetUsers_UserId",
                schema: "Tasks",
                table: "InternalTaskAssignment",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_InternalTaskAssignment_InternalTask_InternalTaskId",
                schema: "Tasks",
                table: "InternalTaskAssignment",
                column: "InternalTaskId",
                principalSchema: "Tasks",
                principalTable: "InternalTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobDescription_AspNetRoles_RoleId",
                schema: "HR",
                table: "JobDescription",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobResponsibility_JobDescription_JobDescriptionId",
                schema: "HR",
                table: "JobResponsibility",
                column: "JobDescriptionId",
                principalSchema: "HR",
                principalTable: "JobDescription",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KPIIncedint_AspNetUsers_EmployeeId",
                schema: "HR",
                table: "KPIIncedint",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadNote_AspNetUsers_CreatedById",
                schema: "CRM",
                table: "LeadNote",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeadNote_SalesLead_LeadId",
                schema: "CRM",
                table: "LeadNote",
                column: "LeadId",
                principalSchema: "CRM",
                principalTable: "SalesLead",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadServices_SalesLead_LeadId",
                schema: "CRM",
                table: "LeadServices",
                column: "LeadId",
                principalSchema: "CRM",
                principalTable: "SalesLead",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadServices_Service_ServiceId",
                schema: "CRM",
                table: "LeadServices",
                column: "ServiceId",
                principalSchema: "Services",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequest_AspNetUsers_EmployeeId",
                schema: "HR",
                table: "LeaveRequest",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryPayment_AspNetUsers_EmployeeId",
                schema: "HR",
                table: "SalaryPayment",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryPayment_SalaryPeriod_SalaryPeriodId",
                schema: "HR",
                table: "SalaryPayment",
                column: "SalaryPeriodId",
                principalSchema: "HR",
                principalTable: "SalaryPeriod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryPeriod_AspNetUsers_EmployeeId",
                schema: "HR",
                table: "SalaryPeriod",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesLead_AspNetUsers_CreatedById",
                schema: "CRM",
                table: "SalesLead",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesLead_AspNetUsers_SalesRepId",
                schema: "CRM",
                table: "SalesLead",
                column: "SalesRepId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCheckpoint_Service_ServiceId",
                schema: "Services",
                table: "ServiceCheckpoint",
                column: "ServiceId",
                principalSchema: "Services",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskComment_AspNetUsers_EmployeeId",
                schema: "Tasks",
                table: "TaskComment",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskComment_TaskItem_TaskId",
                schema: "Tasks",
                table: "TaskComment",
                column: "TaskId",
                principalSchema: "Tasks",
                principalTable: "TaskItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCompletionResources_ArchivedTask_ArchivedTaskId",
                schema: "Tasks",
                table: "TaskCompletionResources",
                column: "ArchivedTaskId",
                principalSchema: "Tasks",
                principalTable: "ArchivedTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCompletionResources_TaskItem_TaskId",
                schema: "Tasks",
                table: "TaskCompletionResources",
                column: "TaskId",
                principalSchema: "Tasks",
                principalTable: "TaskItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskGroup_ClientService_ClientServiceId",
                schema: "Tasks",
                table: "TaskGroup",
                column: "ClientServiceId",
                principalSchema: "Clients",
                principalTable: "ClientService",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItem_AspNetUsers_EmployeeId",
                schema: "Tasks",
                table: "TaskItem",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItem_ClientService_ClientServiceId",
                schema: "Tasks",
                table: "TaskItem",
                column: "ClientServiceId",
                principalSchema: "Clients",
                principalTable: "ClientService",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItem_TaskGroup_TaskGroupId",
                schema: "Tasks",
                table: "TaskItem",
                column: "TaskGroupId",
                principalSchema: "Tasks",
                principalTable: "TaskGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItemHistory_ArchivedTask_ArchivedTaskId",
                schema: "Tasks",
                table: "TaskItemHistory",
                column: "ArchivedTaskId",
                principalSchema: "Tasks",
                principalTable: "ArchivedTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItemHistory_AspNetUsers_UpdatedById",
                schema: "Tasks",
                table: "TaskItemHistory",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItemHistory_TaskItem_TaskItemId",
                schema: "Tasks",
                table: "TaskItemHistory",
                column: "TaskItemId",
                principalSchema: "Tasks",
                principalTable: "TaskItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementLink_Announcement_AnnouncementId",
                schema: "Communication",
                table: "AnnouncementLink");

            migrationBuilder.DropForeignKey(
                name: "FK_ArchivedTask_AspNetUsers_EmployeeId",
                schema: "Tasks",
                table: "ArchivedTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ArchivedTask_ClientService_ClientServiceId",
                schema: "Tasks",
                table: "ArchivedTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ArchivedTask_TaskGroup_TaskGroupId",
                schema: "Tasks",
                table: "ArchivedTask");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecord_AspNetUsers_EmployeeId",
                schema: "HR",
                table: "AttendanceRecord");

            migrationBuilder.DropForeignKey(
                name: "FK_BreakPeriod_AttendanceRecord_AttendanceRecordId",
                schema: "HR",
                table: "BreakPeriod");

            migrationBuilder.DropForeignKey(
                name: "FK_Client_AspNetUsers_AccountManagerId",
                schema: "Clients",
                table: "Client");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientService_Client_ClientId",
                schema: "Clients",
                table: "ClientService");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientService_Service_ServiceId",
                schema: "Clients",
                table: "ClientService");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientServiceCheckpoint_AspNetUsers_CompletedByEmployeeId",
                schema: "Clients",
                table: "ClientServiceCheckpoint");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientServiceCheckpoint_ClientService_ClientServiceId",
                schema: "Clients",
                table: "ClientServiceCheckpoint");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientServiceCheckpoint_ServiceCheckpoint_ServiceCheckpointId",
                schema: "Clients",
                table: "ClientServiceCheckpoint");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaint_AspNetUsers_EmployeeId",
                schema: "Communication",
                table: "Complaint");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomContract_Client_ClientId",
                schema: "Clients",
                table: "CustomContract");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomContract_Currencies_CurrencyId",
                schema: "Clients",
                table: "CustomContract");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeOnBoardingInvitation_AspNetUsers_InvitedByUserId",
                schema: "Identity",
                table: "EmployeeOnBoardingInvitation");

            migrationBuilder.DropForeignKey(
                name: "FK_InternalTaskAssignment_AspNetUsers_UserId",
                schema: "Tasks",
                table: "InternalTaskAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_InternalTaskAssignment_InternalTask_InternalTaskId",
                schema: "Tasks",
                table: "InternalTaskAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_JobDescription_AspNetRoles_RoleId",
                schema: "HR",
                table: "JobDescription");

            migrationBuilder.DropForeignKey(
                name: "FK_JobResponsibility_JobDescription_JobDescriptionId",
                schema: "HR",
                table: "JobResponsibility");

            migrationBuilder.DropForeignKey(
                name: "FK_KPIIncedint_AspNetUsers_EmployeeId",
                schema: "HR",
                table: "KPIIncedint");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadNote_AspNetUsers_CreatedById",
                schema: "CRM",
                table: "LeadNote");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadNote_SalesLead_LeadId",
                schema: "CRM",
                table: "LeadNote");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadServices_SalesLead_LeadId",
                schema: "CRM",
                table: "LeadServices");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadServices_Service_ServiceId",
                schema: "CRM",
                table: "LeadServices");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequest_AspNetUsers_EmployeeId",
                schema: "HR",
                table: "LeaveRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_SalaryPayment_AspNetUsers_EmployeeId",
                schema: "HR",
                table: "SalaryPayment");

            migrationBuilder.DropForeignKey(
                name: "FK_SalaryPayment_SalaryPeriod_SalaryPeriodId",
                schema: "HR",
                table: "SalaryPayment");

            migrationBuilder.DropForeignKey(
                name: "FK_SalaryPeriod_AspNetUsers_EmployeeId",
                schema: "HR",
                table: "SalaryPeriod");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesLead_AspNetUsers_CreatedById",
                schema: "CRM",
                table: "SalesLead");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesLead_AspNetUsers_SalesRepId",
                schema: "CRM",
                table: "SalesLead");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCheckpoint_Service_ServiceId",
                schema: "Services",
                table: "ServiceCheckpoint");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskComment_AspNetUsers_EmployeeId",
                schema: "Tasks",
                table: "TaskComment");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskComment_TaskItem_TaskId",
                schema: "Tasks",
                table: "TaskComment");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCompletionResources_ArchivedTask_ArchivedTaskId",
                schema: "Tasks",
                table: "TaskCompletionResources");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCompletionResources_TaskItem_TaskId",
                schema: "Tasks",
                table: "TaskCompletionResources");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskGroup_ClientService_ClientServiceId",
                schema: "Tasks",
                table: "TaskGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItem_AspNetUsers_EmployeeId",
                schema: "Tasks",
                table: "TaskItem");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItem_ClientService_ClientServiceId",
                schema: "Tasks",
                table: "TaskItem");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItem_TaskGroup_TaskGroupId",
                schema: "Tasks",
                table: "TaskItem");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItemHistory_ArchivedTask_ArchivedTaskId",
                schema: "Tasks",
                table: "TaskItemHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItemHistory_AspNetUsers_UpdatedById",
                schema: "Tasks",
                table: "TaskItemHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItemHistory_TaskItem_TaskItemId",
                schema: "Tasks",
                table: "TaskItemHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskItemHistory",
                schema: "Tasks",
                table: "TaskItemHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskItem",
                schema: "Tasks",
                table: "TaskItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskGroup",
                schema: "Tasks",
                table: "TaskGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskComment",
                schema: "Tasks",
                table: "TaskComment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceCheckpoint",
                schema: "Services",
                table: "ServiceCheckpoint");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Service",
                schema: "Services",
                table: "Service");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SeoContent",
                schema: "Content",
                table: "SeoContent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SalesLead",
                schema: "CRM",
                table: "SalesLead");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SalaryPeriod",
                schema: "HR",
                table: "SalaryPeriod");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SalaryPayment",
                schema: "HR",
                table: "SalaryPayment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveRequest",
                schema: "HR",
                table: "LeaveRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeadNote",
                schema: "CRM",
                table: "LeadNote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KPIIncedint",
                schema: "HR",
                table: "KPIIncedint");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobResponsibility",
                schema: "HR",
                table: "JobResponsibility");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobDescription",
                schema: "HR",
                table: "JobDescription");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InternalTaskAssignment",
                schema: "Tasks",
                table: "InternalTaskAssignment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InternalTask",
                schema: "Tasks",
                table: "InternalTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeOnBoardingInvitation",
                schema: "Identity",
                table: "EmployeeOnBoardingInvitation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomContract",
                schema: "Clients",
                table: "CustomContract");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactEntry",
                schema: "CRM",
                table: "ContactEntry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Complaint",
                schema: "Communication",
                table: "Complaint");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientServiceCheckpoint",
                schema: "Clients",
                table: "ClientServiceCheckpoint");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientService",
                schema: "Clients",
                table: "ClientService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Client",
                schema: "Clients",
                table: "Client");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BreakPeriod",
                schema: "HR",
                table: "BreakPeriod");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttendanceRecord",
                schema: "HR",
                table: "AttendanceRecord");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ArchivedTask",
                schema: "Tasks",
                table: "ArchivedTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnnouncementLink",
                schema: "Communication",
                table: "AnnouncementLink");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Announcement",
                schema: "Communication",
                table: "Announcement");

            migrationBuilder.RenameTable(
                name: "TaskCompletionResources",
                schema: "Tasks",
                newName: "TaskCompletionResources");

            migrationBuilder.RenameTable(
                name: "LeadServices",
                schema: "CRM",
                newName: "LeadServices");

            migrationBuilder.RenameTable(
                name: "TaskItemHistory",
                schema: "Tasks",
                newName: "TaskHistory");

            migrationBuilder.RenameTable(
                name: "TaskItem",
                schema: "Tasks",
                newName: "Tasks");

            migrationBuilder.RenameTable(
                name: "TaskGroup",
                schema: "Tasks",
                newName: "TaskGroups");

            migrationBuilder.RenameTable(
                name: "TaskComment",
                schema: "Tasks",
                newName: "TaskComments");

            migrationBuilder.RenameTable(
                name: "ServiceCheckpoint",
                schema: "Services",
                newName: "ServiceCheckpoints");

            migrationBuilder.RenameTable(
                name: "Service",
                schema: "Services",
                newName: "Services");

            migrationBuilder.RenameTable(
                name: "SeoContent",
                schema: "Content",
                newName: "SeoContents");

            migrationBuilder.RenameTable(
                name: "SalesLead",
                schema: "CRM",
                newName: "SalesLeads");

            migrationBuilder.RenameTable(
                name: "SalaryPeriod",
                schema: "HR",
                newName: "SalaryPeriods");

            migrationBuilder.RenameTable(
                name: "SalaryPayment",
                schema: "HR",
                newName: "SalaryPayments");

            migrationBuilder.RenameTable(
                name: "LeaveRequest",
                schema: "HR",
                newName: "LeaveRequests");

            migrationBuilder.RenameTable(
                name: "LeadNote",
                schema: "CRM",
                newName: "LeadNotes");

            migrationBuilder.RenameTable(
                name: "KPIIncedint",
                schema: "HR",
                newName: "KPIIncedints");

            migrationBuilder.RenameTable(
                name: "JobResponsibility",
                schema: "HR",
                newName: "JobResponsibilities");

            migrationBuilder.RenameTable(
                name: "JobDescription",
                schema: "HR",
                newName: "JobDescriptions");

            migrationBuilder.RenameTable(
                name: "InternalTaskAssignment",
                schema: "Tasks",
                newName: "InternalTaskAssignments");

            migrationBuilder.RenameTable(
                name: "InternalTask",
                schema: "Tasks",
                newName: "InternalTasks");

            migrationBuilder.RenameTable(
                name: "EmployeeOnBoardingInvitation",
                schema: "Identity",
                newName: "EmployeeOnBoardingInvitations");

            migrationBuilder.RenameTable(
                name: "CustomContract",
                schema: "Clients",
                newName: "Contracts");

            migrationBuilder.RenameTable(
                name: "ContactEntry",
                schema: "CRM",
                newName: "ContactEntries");

            migrationBuilder.RenameTable(
                name: "Complaint",
                schema: "Communication",
                newName: "Complaints");

            migrationBuilder.RenameTable(
                name: "ClientServiceCheckpoint",
                schema: "Clients",
                newName: "ClientServiceCheckpoints");

            migrationBuilder.RenameTable(
                name: "ClientService",
                schema: "Clients",
                newName: "ClientServices");

            migrationBuilder.RenameTable(
                name: "Client",
                schema: "Clients",
                newName: "Clients");

            migrationBuilder.RenameTable(
                name: "BreakPeriod",
                schema: "HR",
                newName: "BreakPeriods");

            migrationBuilder.RenameTable(
                name: "AttendanceRecord",
                schema: "HR",
                newName: "AttendanceRecords");

            migrationBuilder.RenameTable(
                name: "ArchivedTask",
                schema: "Tasks",
                newName: "ArchivedTasks");

            migrationBuilder.RenameTable(
                name: "AnnouncementLink",
                schema: "Communication",
                newName: "AnnouncementLinks");

            migrationBuilder.RenameTable(
                name: "Announcement",
                schema: "Communication",
                newName: "Announcements");

            migrationBuilder.RenameIndex(
                name: "IX_TaskItemHistory_UpdatedById",
                table: "TaskHistory",
                newName: "IX_TaskHistory_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_TaskItemHistory_TaskItemId",
                table: "TaskHistory",
                newName: "IX_TaskHistory_TaskItemId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskItemHistory_ArchivedTaskId",
                table: "TaskHistory",
                newName: "IX_TaskHistory_ArchivedTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskItem_TaskGroupId",
                table: "Tasks",
                newName: "IX_Tasks_TaskGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskItem_EmployeeId",
                table: "Tasks",
                newName: "IX_Tasks_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskItem_ClientServiceId",
                table: "Tasks",
                newName: "IX_Tasks_ClientServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskGroup_ClientServiceId",
                table: "TaskGroups",
                newName: "IX_TaskGroups_ClientServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskComment_TaskId",
                table: "TaskComments",
                newName: "IX_TaskComments_TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskComment_EmployeeId",
                table: "TaskComments",
                newName: "IX_TaskComments_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceCheckpoint_ServiceId_Order",
                table: "ServiceCheckpoints",
                newName: "IX_ServiceCheckpoints_ServiceId_Order");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceCheckpoint_ServiceId",
                table: "ServiceCheckpoints",
                newName: "IX_ServiceCheckpoints_ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Service_Title",
                table: "Services",
                newName: "IX_Services_Title");

            migrationBuilder.RenameIndex(
                name: "IX_SeoContent_Route_Language",
                table: "SeoContents",
                newName: "IX_SeoContents_Route_Language");

            migrationBuilder.RenameIndex(
                name: "IX_SalesLead_SalesRepId",
                table: "SalesLeads",
                newName: "IX_SalesLeads_SalesRepId");

            migrationBuilder.RenameIndex(
                name: "IX_SalesLead_CreatedById",
                table: "SalesLeads",
                newName: "IX_SalesLeads_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_SalesLead_CreatedAt",
                table: "SalesLeads",
                newName: "IX_SalesLeads_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_SalaryPeriod_MonthLabel",
                table: "SalaryPeriods",
                newName: "IX_SalaryPeriods_MonthLabel");

            migrationBuilder.RenameIndex(
                name: "IX_SalaryPeriod_EmployeeId_Year_Month",
                table: "SalaryPeriods",
                newName: "IX_SalaryPeriods_EmployeeId_Year_Month");

            migrationBuilder.RenameIndex(
                name: "IX_SalaryPeriod_EmployeeId",
                table: "SalaryPeriods",
                newName: "IX_SalaryPeriods_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_SalaryPayment_SalaryPeriodId",
                table: "SalaryPayments",
                newName: "IX_SalaryPayments_SalaryPeriodId");

            migrationBuilder.RenameIndex(
                name: "IX_SalaryPayment_EmployeeId",
                table: "SalaryPayments",
                newName: "IX_SalaryPayments_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_LeaveRequest_EmployeeId",
                table: "LeaveRequests",
                newName: "IX_LeaveRequests_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_LeadNote_LeadId",
                table: "LeadNotes",
                newName: "IX_LeadNotes_LeadId");

            migrationBuilder.RenameIndex(
                name: "IX_LeadNote_CreatedById",
                table: "LeadNotes",
                newName: "IX_LeadNotes_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_KPIIncedint_EmployeeId",
                table: "KPIIncedints",
                newName: "IX_KPIIncedints_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_JobResponsibility_JobDescriptionId",
                table: "JobResponsibilities",
                newName: "IX_JobResponsibilities_JobDescriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_JobDescription_RoleId",
                table: "JobDescriptions",
                newName: "IX_JobDescriptions_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_InternalTaskAssignment_UserId",
                table: "InternalTaskAssignments",
                newName: "IX_InternalTaskAssignments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_InternalTaskAssignment_InternalTaskId",
                table: "InternalTaskAssignments",
                newName: "IX_InternalTaskAssignments_InternalTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeOnBoardingInvitation_InvitedByUserId",
                table: "EmployeeOnBoardingInvitations",
                newName: "IX_EmployeeOnBoardingInvitations_InvitedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomContract_CurrencyId",
                table: "Contracts",
                newName: "IX_Contracts_CurrencyId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomContract_ClientId",
                table: "Contracts",
                newName: "IX_Contracts_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_Complaint_EmployeeId",
                table: "Complaints",
                newName: "IX_Complaints_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientServiceCheckpoint_ServiceCheckpointId",
                table: "ClientServiceCheckpoints",
                newName: "IX_ClientServiceCheckpoints_ServiceCheckpointId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientServiceCheckpoint_CompletedByEmployeeId",
                table: "ClientServiceCheckpoints",
                newName: "IX_ClientServiceCheckpoints_CompletedByEmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientServiceCheckpoint_ClientServiceId_ServiceCheckpointId",
                table: "ClientServiceCheckpoints",
                newName: "IX_ClientServiceCheckpoints_ClientServiceId_ServiceCheckpointId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientServiceCheckpoint_ClientServiceId",
                table: "ClientServiceCheckpoints",
                newName: "IX_ClientServiceCheckpoints_ClientServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientService_ServiceId",
                table: "ClientServices",
                newName: "IX_ClientServices_ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientService_ClientId",
                table: "ClientServices",
                newName: "IX_ClientServices_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_Client_Name",
                table: "Clients",
                newName: "IX_Clients_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Client_CompanyName",
                table: "Clients",
                newName: "IX_Clients_CompanyName");

            migrationBuilder.RenameIndex(
                name: "IX_Client_AccountManagerId",
                table: "Clients",
                newName: "IX_Clients_AccountManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_BreakPeriod_AttendanceRecordId",
                table: "BreakPeriods",
                newName: "IX_BreakPeriods_AttendanceRecordId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecord_EmployeeId_WorkDate",
                table: "AttendanceRecords",
                newName: "IX_AttendanceRecords_EmployeeId_WorkDate");

            migrationBuilder.RenameIndex(
                name: "IX_ArchivedTask_TaskGroupId",
                table: "ArchivedTasks",
                newName: "IX_ArchivedTasks_TaskGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_ArchivedTask_EmployeeId",
                table: "ArchivedTasks",
                newName: "IX_ArchivedTasks_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_ArchivedTask_ClientServiceId",
                table: "ArchivedTasks",
                newName: "IX_ArchivedTasks_ClientServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_AnnouncementLink_AnnouncementId",
                table: "AnnouncementLinks",
                newName: "IX_AnnouncementLinks_AnnouncementId");

            migrationBuilder.RenameIndex(
                name: "IX_Announcement_CreatedAt",
                table: "Announcements",
                newName: "IX_Announcements_CreatedAt");

            migrationBuilder.AlterColumn<int>(
                name: "TaskGroupId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TaskGroupId",
                table: "ArchivedTasks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskHistory",
                table: "TaskHistory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskGroups",
                table: "TaskGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskComments",
                table: "TaskComments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceCheckpoints",
                table: "ServiceCheckpoints",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Services",
                table: "Services",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SeoContents",
                table: "SeoContents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SalesLeads",
                table: "SalesLeads",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SalaryPeriods",
                table: "SalaryPeriods",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SalaryPayments",
                table: "SalaryPayments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveRequests",
                table: "LeaveRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeadNotes",
                table: "LeadNotes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KPIIncedints",
                table: "KPIIncedints",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobResponsibilities",
                table: "JobResponsibilities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobDescriptions",
                table: "JobDescriptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InternalTaskAssignments",
                table: "InternalTaskAssignments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InternalTasks",
                table: "InternalTasks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeOnBoardingInvitations",
                table: "EmployeeOnBoardingInvitations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contracts",
                table: "Contracts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactEntries",
                table: "ContactEntries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Complaints",
                table: "Complaints",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientServiceCheckpoints",
                table: "ClientServiceCheckpoints",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientServices",
                table: "ClientServices",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clients",
                table: "Clients",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BreakPeriods",
                table: "BreakPeriods",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttendanceRecords",
                table: "AttendanceRecords",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArchivedTasks",
                table: "ArchivedTasks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnnouncementLinks",
                table: "AnnouncementLinks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Announcements",
                table: "Announcements",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementLinks_Announcements_AnnouncementId",
                table: "AnnouncementLinks",
                column: "AnnouncementId",
                principalTable: "Announcements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArchivedTasks_AspNetUsers_EmployeeId",
                table: "ArchivedTasks",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ArchivedTasks_ClientServices_ClientServiceId",
                table: "ArchivedTasks",
                column: "ClientServiceId",
                principalTable: "ClientServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ArchivedTasks_TaskGroups_TaskGroupId",
                table: "ArchivedTasks",
                column: "TaskGroupId",
                principalTable: "TaskGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_AspNetUsers_EmployeeId",
                table: "AttendanceRecords",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BreakPeriods_AttendanceRecords_AttendanceRecordId",
                table: "BreakPeriods",
                column: "AttendanceRecordId",
                principalTable: "AttendanceRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_AspNetUsers_AccountManagerId",
                table: "Clients",
                column: "AccountManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientServiceCheckpoints_AspNetUsers_CompletedByEmployeeId",
                table: "ClientServiceCheckpoints",
                column: "CompletedByEmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientServiceCheckpoints_ClientServices_ClientServiceId",
                table: "ClientServiceCheckpoints",
                column: "ClientServiceId",
                principalTable: "ClientServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientServiceCheckpoints_ServiceCheckpoints_ServiceCheckpointId",
                table: "ClientServiceCheckpoints",
                column: "ServiceCheckpointId",
                principalTable: "ServiceCheckpoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientServices_Clients_ClientId",
                table: "ClientServices",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientServices_Services_ServiceId",
                table: "ClientServices",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_AspNetUsers_EmployeeId",
                table: "Complaints",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Clients_ClientId",
                table: "Contracts",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Currencies_CurrencyId",
                table: "Contracts",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeOnBoardingInvitations_AspNetUsers_InvitedByUserId",
                table: "EmployeeOnBoardingInvitations",
                column: "InvitedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InternalTaskAssignments_AspNetUsers_UserId",
                table: "InternalTaskAssignments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_InternalTaskAssignments_InternalTasks_InternalTaskId",
                table: "InternalTaskAssignments",
                column: "InternalTaskId",
                principalTable: "InternalTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobDescriptions_AspNetRoles_RoleId",
                table: "JobDescriptions",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobResponsibilities_JobDescriptions_JobDescriptionId",
                table: "JobResponsibilities",
                column: "JobDescriptionId",
                principalTable: "JobDescriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KPIIncedints_AspNetUsers_EmployeeId",
                table: "KPIIncedints",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadNotes_AspNetUsers_CreatedById",
                table: "LeadNotes",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeadNotes_SalesLeads_LeadId",
                table: "LeadNotes",
                column: "LeadId",
                principalTable: "SalesLeads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadServices_SalesLeads_LeadId",
                table: "LeadServices",
                column: "LeadId",
                principalTable: "SalesLeads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadServices_Services_ServiceId",
                table: "LeadServices",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_EmployeeId",
                table: "LeaveRequests",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryPayments_AspNetUsers_EmployeeId",
                table: "SalaryPayments",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryPayments_SalaryPeriods_SalaryPeriodId",
                table: "SalaryPayments",
                column: "SalaryPeriodId",
                principalTable: "SalaryPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryPeriods_AspNetUsers_EmployeeId",
                table: "SalaryPeriods",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesLeads_AspNetUsers_CreatedById",
                table: "SalesLeads",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesLeads_AspNetUsers_SalesRepId",
                table: "SalesLeads",
                column: "SalesRepId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCheckpoints_Services_ServiceId",
                table: "ServiceCheckpoints",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskComments_AspNetUsers_EmployeeId",
                table: "TaskComments",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskComments_Tasks_TaskId",
                table: "TaskComments",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCompletionResources_ArchivedTasks_ArchivedTaskId",
                table: "TaskCompletionResources",
                column: "ArchivedTaskId",
                principalTable: "ArchivedTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCompletionResources_Tasks_TaskId",
                table: "TaskCompletionResources",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskGroups_ClientServices_ClientServiceId",
                table: "TaskGroups",
                column: "ClientServiceId",
                principalTable: "ClientServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistory_ArchivedTasks_ArchivedTaskId",
                table: "TaskHistory",
                column: "ArchivedTaskId",
                principalTable: "ArchivedTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistory_AspNetUsers_UpdatedById",
                table: "TaskHistory",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistory_Tasks_TaskItemId",
                table: "TaskHistory",
                column: "TaskItemId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_EmployeeId",
                table: "Tasks",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_ClientServices_ClientServiceId",
                table: "Tasks",
                column: "ClientServiceId",
                principalTable: "ClientServices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskGroups_TaskGroupId",
                table: "Tasks",
                column: "TaskGroupId",
                principalTable: "TaskGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
