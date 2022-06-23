using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationProject.DAL.MigrationExtentions
{
    public static class MigrationExtentions
    {
        public static void SP_JobStoredProcedureUp(this MigrationBuilder migrationBuilder) 
        {
			var sql = @"
			create procedure GenerateBills
			as
			begin
			
				declare @TenantId int
				declare @RentStart datetime
				declare @RentEnd datetime
				declare @RentId int
				declare @Pricing real
			
				declare MyCursor cursor 
				for (select Rents.TenantId, Rents.RentStart, Rents.RentEnd, Rents.Id, RoomTypes.Pricing from Rents
					 join Rooms on Rooms.Id = Rents.RoomId
					 join RoomTypes on RoomTypes.Id = Rooms.TypeId
					 where Rents.BillCreated = 0 and Rents.RentEnd < getdate())

				open MyCursor		

				fetch next from MyCursor into 
					@TenantId,
					@RentStart,
					@RentEnd,
					@RentId,
					@Pricing
				
				while @@FETCH_STATUS = 0
				begin
					declare @Ammount real = @Pricing * datediff(month, @RentStart, @RentEnd)
			
					insert into Bills (Ammount, Tax, CreationDate, PaymentDeadline, FinePerDay, RentId, TenantId)
					values(@Ammount, 0.2, getdate(), dateadd(day,15,getdate()), 0.002, @RentId, @TenantId)
			
					fetch next from MyCursor into 
						@TenantId,
						@RentStart,
						@RentEnd,
						@RentId,
						@Pricing
				end

				close MyCursor
			
				insert into Bills (Ammount, Tax, CreationDate, PaymentDeadline, FinePerDay, RentId, TenantId)
				select Ammount * datediff(day, PaymentDeadline, getdate()), Tax, getdate(), cast('12/31/9999 00:00:00.000' as datetime), 0, RentId, TenantId  from Bills
				where PaymentDeadline < getdate() and PaymentDate = null
			
			end";

			migrationBuilder.Sql(sql);
		}

        public static void SP_JobStoredProcedureDown(this MigrationBuilder migrationBuilder)
        {
			var sql = @"
			DROP PROCEDURE GenerateBills
			";

			migrationBuilder.Sql(sql);
		}

		public static void AddDefaultAdminAndRolesUp(this MigrationBuilder migrationBuilder)
		{
			var sql = @"
            CREATE LOGIN DefaultAdmin WITH PASSWORD = 'DefaultAdmin'
            
            CREATE USER DefaultAdmin FOR LOGIN DefaultAdmin

            CREATE ROLE AdminRole 

            GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA :: dbo TO AdminRole

            GRANT ALTER ANY USER TO DefaultAdmin WITH GRANT OPTION

            GRANT ALTER ANY ROLE TO DefaultAdmin WITH GRANT OPTION
            
            DECLARE @sql nvarchar(MAX) = '
    
                USE master
                GRANT ALTER ANY LOGIN TO DefaultAdmin WITH GRANT OPTION
'
            
            EXEC(@sql)

            ALTER ROLE AdminRole ADD MEMBER DefaultAdmin

            CREATE ROLE TenantRole 

            GRANT SELECT ON SCHEMA :: dbo TO TenantRole

            INSERT INTO Admins (Username)
            VALUES('DefaultAdmin')
            ";

			migrationBuilder.Sql(sql);
		}

		public static void AddDefaultAdminAndRolesDown(this MigrationBuilder migrationBuilder)
		{
			var sql = @"

            IF EXISTS 
                (SELECT name  
                 FROM master.sys.server_principals
                 WHERE name = 'LoginName')
            DROP LOGIN DefaultAdmin

            DROP USER IF EXISTS DefaultAdmin

            DROP ROLE TenantRole

            DROP ROLE AdminRole
            
            DELETE FROM Admins
            WHERE Username = 'DefaultAdmin'
            ";

			migrationBuilder.Sql(sql);
		}

		public static void ConfigureAdvancedOpdionsAndAgentUp(this MigrationBuilder migrationBuilder)
		{
			var sql = @"
			EXEC Sp_configure 
              'Show advanced options', 
              1 
            
            RECONFIGURE WITH override 
            
            EXEC Sp_configure 
              'Agent XPs', 
              1 
            
            RECONFIGURE WITH override 
			";

			migrationBuilder.Sql(sql, suppressTransaction: true);
		}

		public static void ConfigureAdvancedOpdionsAndAgentDown(this MigrationBuilder migrationBuilder)
		{
			var sql = @"
			EXEC Sp_configure 
              'Agent XPs', 
              0 
            
            RECONFIGURE WITH override 


            EXEC Sp_configure 
              'Show advanced options', 
              0 
            
            RECONFIGURE WITH override 
			";

			migrationBuilder.Sql(sql, suppressTransaction: true);
		}

		public static void AddJobUp(this MigrationBuilder migrationBuilder)
		{
			var sql = @"
            exec msdb.dbo.sp_add_job
            	@job_name=N'Job', 
            	@enabled=1
            
            exec msdb.dbo.sp_add_jobserver 
              @job_name=N'Job'
            
            declare @Database nvarchar(MAX) = db_name()
            
            exec msdb.dbo.sp_add_jobstep  
                @job_name = N'Job',  
                @step_name = N'Step',  
                @subsystem = N'TSQL',  
            	@database_name = @Database,
                @command = N'EXEC AddRow',   
                @retry_attempts = 0,  
                @retry_interval = 5 
            
            
            exec msdb.dbo.sp_add_schedule  
                @schedule_name = N'RunEveryMonth',  
            	@enabled = 1,
                @freq_type = 16,  
            	@freq_interval = 1,
            	@freq_recurrence_factor = 1
            
            exec msdb.dbo.sp_attach_schedule  
               @job_name = N'Job',  
               @schedule_name = N'RunEveryMonth'  
               ";

			migrationBuilder.Sql(sql);
		}

		public static void AddJobDown(this MigrationBuilder migrationBuilder)
		{
			var sql = @"
            exec msdb.dbo.sp_delete_job  
	            @job_name=N'Job'
            ";

			migrationBuilder.Sql(sql);
		}

		public static void SP_RegisterUp(this MigrationBuilder migrationBuilder)
		{
			var sql = @"
            create procedure Register
			@Username nvarchar(MAX),
			@Password nvarchar(MAX),
			@Name nvarchar(MAX),
			@LegalAddress nvarchar(MAX),
			@BankName nvarchar(MAX),
			@Head nvarchar(MAX),
			@Characteristic nvarchar(MAX),
			@Result nvarchar(MAX) output
			with execute as CALLER
			as 
			begin
				declare @AdminCount int
			
				set @AdminCount = (select count(Username) from Admins
								   where Admins.Username = @Username)
			
				declare @TenantCount int 
				
				set @TenantCount = (select count(Username) from Tenants
									where Tenants.Username = @Username)
			
				if @AdminCount + @TenantCount = 0
				begin
			
					begin transaction TCreateTenant
			
					begin try
			
						DECLARE @DynamicSQL nvarchar(MAX) = 'CREATE LOGIN ' + QUOTENAME(@Username) + ' WITH PASSWORD = ' + QUOTENAME(@Password,'''') 
			
						EXEC (@DynamicSQL)
			
						SET @DynamicSQL = 'CREATE USER ' + QUOTENAME(@Username) + ' FOR LOGIN ' + QUOTENAME(@Username)
			
						EXEC (@DynamicSQL)
			
						SET @DynamicSQL = 'ALTER ROLE TenantRole ADD MEMBER ' + QUOTENAME(@Username)
			
						EXEC (@DynamicSQL)
			
						insert into Tenants (Name, LegalAddress, BankName, Head, Characteristic, Username, HasAccess)
						values(@Name, @LegalAddress, @BankName, @Head, @Characteristic, @Username, 1)
			
					end try
					begin catch 
						select @Result = 'Uknown error during user creation'
						rollback transaction TCreateTenant

						return
					end catch
					select @Result = 'Success'
			
					commit transaction TCreateTenant
			
				end
				else
				select @Result = 'Username is already taken'
			
			end
            ";

			migrationBuilder.Sql(sql);
		}

		public static void SP_RegisterDown(this MigrationBuilder migrationBuilder)
		{
			var sql = @"
            drop procedure Register
            ";

			migrationBuilder.Sql(sql);
		}

		public static void AddRegistrationUserUp(this MigrationBuilder migrationBuilder)
		{
			var sql = @"
            CREATE LOGIN Registration WITH PASSWORD = 'Registration'
            
            CREATE USER Registration FOR LOGIN Registration

            GRANT EXECUTE ON OBJECT::Register TO Registration
            ";

			migrationBuilder.Sql(sql);
		}

		public static void AddRegistrationUserDown(this MigrationBuilder migrationBuilder)
		{
			var sql = @"
            DROP LOGIN Registration

            DROP USER Registration
            ";

			migrationBuilder.Sql(sql);
		}

		public static void DropUsers(this MigrationBuilder migrationBuilder)
		{
			var sql = @"
			declare @User nvarchar(MAX)

            declare MyCursor cursor 
				for (select Username from Tenants
					 union 
					 select Username from Admins)

				open MyCursor		

				fetch next from MyCursor into 
					@User
				
				while @@FETCH_STATUS = 0
				begin
					
					DECLARE @DynamicSQL nvarchar(MAX) = 'DROP USER ' + QUOTENAME(@User)
			
					EXEC (@DynamicSQL)

					SET @DynamicSQL = 'DROP LOGIN ' +  QUOTENAME(@User)

					EXEC (@DynamicSQL)
			
					fetch next from MyCursor into 
					@User
				end

			close MyCursor
            ";

			migrationBuilder.Sql(sql);
		}

		public static void CreateCertificateAndSignRegisterProcedureUp(this MigrationBuilder migrationBuilder)
		{
			var sql = @"
            CREATE CERTIFICATE register_certificate 
                ENCRYPTION BY PASSWORD = 'pGFD4bb925DGvbd2439587y'
                WITH SUBJECT = 'NoSubject'

            ADD SIGNATURE TO Register
                BY CERTIFICATE register_certificate WITH PASSWORD = 'pGFD4bb925DGvbd2439587y'

            ALTER CERTIFICATE register_certificate REMOVE PRIVATE KEY

            CREATE USER register_certificate_user
                FROM CERTIFICATE register_certificate

            GRANT ALTER ANY USER TO register_certificate_user

            GRANT ALTER ANY ROLE TO register_certificate_user

            DECLARE @public_key varbinary(MAX) = 
                certencoded(cert_id('register_certificate'))

            declare @sql_comm nvarchar(MAX) = '
	            use master 
	
	            CREATE CERTIFICATE register_certificate
    
	            FROM BINARY = ' + convert(varchar(MAX), @public_key, 1) + '

	            CREATE LOGIN register_certificate_login 
                    FROM CERTIFICATE register_certificate

	            GRANT ALTER ANY LOGIN TO register_certificate_login
	            '

            exec(@sql_comm)
			";

			migrationBuilder.Sql(sql);
		}

		public static void CreateCertificateAndSignRegisterProcedureDown(this MigrationBuilder migrationBuilder)
		{
			var sql = @"
            declare @sql_comm nvarchar(MAX) = '
	            use master 

	            DROP LOGIN register_certificate_login
                
                DROP CERTIFICATE register_certificate
            '
            
            exec(@sql_comm)

            DROP SIGNATURE FROM Register BY CERTIFICATE register_certificate
            DROP USER IF EXISTS register_certificate_user
            DROP CERTIFICATE register_certificate
            ";

			migrationBuilder.Sql(sql);
		}

		public static void SP_ToggleUserAccessUp(this MigrationBuilder migrationBuilder)
		{
			var sql = @"
            create procedure ToggleAccess
            @Username nvarchar(MAX)
            as
            begin
            	if @Username = CURRENT_USER
            	begin
            		RAISERROR ('You cant disable youre own login', 16, 1)
            		return
            	end
            
            	declare @CurrentState int = (select HasAccess from Tenants
            								 where Username = @Username)
            	
            	declare @Operation nvarchar(MAX)
            
            	if @CurrentState = 1
            		set @Operation = 'DISABLE'
            	else
            		set @Operation = 'ENABLE'

                declare @sql nvarchar(MAX) = 'ALTER LOGIN ' + QUOTENAME(@Username) + ' ' + @Operation

                exec(@sql)

                update Tenants
                set HasAccess = 1 - @CurrentState
                where Username = @Username
            end
            ";

			migrationBuilder.Sql(sql);
		}

		public static void SP_ToggleUserAccessDown(this MigrationBuilder migrationBuilder)
		{
			var sql = @"
            drop procedure ToggleAccess
            ";

			migrationBuilder.Sql(sql);
		}

		public static void SP_AddAdminUp(this MigrationBuilder migrationBuilder)
		{
			var sql = @"
            create procedure CreateAdmin
            @Username nvarchar(MAX),
            @Password nvarchar(MAX)
            as
            begin
            	declare @AdminCount int
            	
            	set @AdminCount = (select count(Username) from Admins
            					   where Admins.Username = @Username)
            	
            	declare @TenantCount int 
            	
            	set @TenantCount = (select count(Username) from Tenants
            						where Tenants.Username = @Username)
            	
            	if @AdminCount + @TenantCount = 0
            	begin
                    begin try
                        begin transaction   
            		    DECLARE @sql nvarchar(MAX) = '

            		    CREATE LOGIN '+ QUOTENAME(@Username) +' WITH PASSWORD = '+ QUOTENAME(@Password,'''') +'
                            
                        CREATE USER '+ QUOTENAME(@Username) +' FOR LOGIN '+ QUOTENAME(@Username) +'
            
            		    ALTER ROLE AdminRole ADD MEMBER '+ QUOTENAME(@Username) 

            		    EXEC(@sql)

                        INSERT INTO Admins (Username)
                        VALUES(@Username)
            
                        SET @sql = '
                            GRANT ALTER ANY USER TO ' + QUOTENAME(@Username) + ' WITH GRANT OPTION

                            GRANT ALTER ANY ROLE TO ' + QUOTENAME(@Username) + ' WITH GRANT OPTION

                            USE master
                            GRANT ALTER ANY LOGIN TO ' + QUOTENAME(@Username) + ' WITH GRANT OPTION'

		                EXEC(@sql)

                        commit transaction   
                    end try
                    begin catch
                        rollback transaction
					    DECLARE @Message varchar(MAX) = concat(ERROR_MESSAGE(),concat(' current user: ', CURRENT_USER)),
							@Severity int = ERROR_SEVERITY(),
							@State smallint = ERROR_STATE()
 
					    RAISERROR (@Message, @Severity, @State)
                    end catch
            	end
            	else
            		RAISERROR('Username already taken',16,1)
            end
            ";

			migrationBuilder.Sql(sql);
		}

		public static void SP_AddAdminDown(this MigrationBuilder migrationBuilder)
		{
			var sql = @"
            drop procedure CreateAdmin
            ";

			migrationBuilder.Sql(sql);
		}

		public static void SP_DeleteUserUp(this MigrationBuilder migrationBuilder)
		{
			var sql = @"
			create procedure DeleteUser
			@Username nvarchar(MAX)
			as
			begin
				declare @AdminCount int
				
				set @AdminCount = (select count(Username) from Admins
								   where Admins.Username = @Username)
				
				declare @TenantCount int 
				
				set @TenantCount = (select count(Username) from Tenants
									where Tenants.Username = @Username)
				
				if @AdminCount + @TenantCount = 0
					RAISERROR('No such user exists',16,1)
				else
				begin
					declare @sql nvarchar(MAX) = '
						DROP LOGIN '+ QUOTENAME(@Username) +'
			
						DROP USER '+ QUOTENAME(@Username) 

					exec(@sql)

					if @AdminCount > 0
					begin
						delete from Admins
						where Username = @Username
					end
					else
					begin
						delete from Tenants
						where Username = @Username
					end
				end
			end            
			";

			migrationBuilder.Sql(sql);
		}

		public static void SP_DeleteUserDown(this MigrationBuilder migrationBuilder)
		{
			var sql = @"
            drop procedure DeleteUser
            ";

			migrationBuilder.Sql(sql);
		}

		public static void AllowAdminToUseAdminProceduresUp(this MigrationBuilder migrationBuilder)
		{
			var sql = @"
            GRANT EXECUTE ON OBJECT::ToggleAccess TO AdminRole

            GRANT EXECUTE ON OBJECT::CreateAdmin TO AdminRole

            GRANT EXECUTE ON OBJECT::DeleteUser TO AdminRole
            ";

			migrationBuilder.Sql(sql);
		}

		public static void AllowAdminToUseAdminProceduresDown(this MigrationBuilder migrationBuilder)
		{
			var sql = @"
            REVOKE EXECUTE ON OBJECT::ToggleAccess TO AdminRole

            REVOKE EXECUTE ON OBJECT::CreateAdmin TO AdminRole

            REVOKE EXECUTE ON OBJECT::DeleteUser TO AdminRole
            ";

			migrationBuilder.Sql(sql);
		}
	}
}
