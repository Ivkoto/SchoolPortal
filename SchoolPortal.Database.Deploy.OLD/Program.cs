using DbUp;
using DbUp.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SchoolPortal.Database.Deploy.OLD
{
    public class Program
    {
        [DllImport("System.Net.Security.Native", EntryPoint = "NetSecurityNative_EnsureGssInitialized")]
        internal static extern int EnsureGssInitialized();

        public static int Main(string[] args)
        {
            Probes();

            //return 1;

            Logging.LoggingProvider.InitializeLogger();

            // Workaround for issue: https://github.com/dotnet/SqlClient/issues/1390
            // When containerized .net core 6+ runs on linux, SqlClient breaks
            if (OperatingSystem.IsLinux() &&
                Environment.Version.Major >= 6)
            {
                EnsureGssInitialized();
            }

            try
            {
                var deployByDbUp = DeployByDbUp(args);

                if (deployByDbUp == 0)
                {
                    Seed();
                }

                return deployByDbUp;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An unhandled exception occurred.");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static int DeployByDbUp(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Deploy");
            Console.ResetColor();

            var configuration =
                new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

            var connectionString = configuration.GetSection("ConnectionStrings:DatabaseConnection").Value;

            DropDatabase.For.SqlDatabase(connectionString);
            EnsureDatabase.For.SqlDatabase(connectionString);

            var isDatabaseUpgradeResultSuccessful = true;
            var scriptFuncs = new List<(bool, Func<string, bool>)>() // IMPORTANT Each directory name MUST starts with a letter
            {
                (true, new(e => e.Contains(".Scripts."))),

                (false, new(e => e.Contains(".AlwaysRunScripts.SubDir2."))),
                (false, new(e => e.Contains(".AlwaysRunScripts.SubDir1."))),
            };

            for (int i = 0; i < scriptFuncs.Count; i++)
            {
                var upgraderEngineBuilder =
                    DeployChanges
                        .To
                        .SqlDatabase(connectionString)
                        .WithScriptsAndCodeEmbeddedInAssembly(Assembly.GetExecutingAssembly(), scriptFuncs[i].Item2);

                upgraderEngineBuilder =
                    scriptFuncs[i].Item1 ?
                    upgraderEngineBuilder :
                    upgraderEngineBuilder
                        .JournalTo(new NullJournal());

                var upgrader =
                    upgraderEngineBuilder
                        .LogToConsole()
                        .Build();

                isDatabaseUpgradeResultSuccessful = upgrader.PerformUpgrade().Successful;
                if (!isDatabaseUpgradeResultSuccessful)
                {
                    break;
                }
            }

            Console.ForegroundColor = isDatabaseUpgradeResultSuccessful ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(isDatabaseUpgradeResultSuccessful ? "Successful" : "Failed");
            Console.ResetColor();

            return isDatabaseUpgradeResultSuccessful ? 0 : 1;
        }

        private const string ConnectionString = "Server=.;Database=SchoolPortal-SynonymId-to-Name;User=sa;Password=yourStrong(!)Password;Integrated Security=false;Encrypt=false;TrustServerCertificate=false;";

        private const string SchemaApplications = "[Applications]";

        private const string TableValidityPeriods = $"{SchemaApplications}.[ValidityPeriods]";
        private const string TableCountries = $"{SchemaApplications}.[Countries]";
        private const string TableAreas = $"{SchemaApplications}.[Areas]";
        private const string TableCommunities = $"{SchemaApplications}.[Communities]";
        private const string TableLocations = $"{SchemaApplications}.[Locations]";
        private const string TableSettlements = $"{SchemaApplications}.[Settlements]";
        private const string TableRegions = $"{SchemaApplications}.[Regions]";
        private const string TableInstitutions = $"{SchemaApplications}.[Institutions]";
        private const string TableAddresses = $"{SchemaApplications}.[Addresses]";
        private const string TableInstitutionsAddresses = $"{SchemaApplications}.[InstitutionsAddresses]";
        private const string TableWebsites = $"{SchemaApplications}.[Websites]";
        private const string TableEmails = $"{SchemaApplications}.[Emails]";
        private const string TablePhoneNumbers = $"{SchemaApplications}.[PhoneNumbers]";
        private const string TableProfessionDirectionWrappers = $"{SchemaApplications}.[ProfessionDirectionWrappers]";
        private const string TableProfessionDirections = $"{SchemaApplications}.[ProfessionDirections]";
        private const string TableProfessions = $"{SchemaApplications}.[Professions]";
        private const string TableSpecialities = $"{SchemaApplications}.[Specialities]";
        private const string TableSubjects = $"{SchemaApplications}.[Subjects]";
        private const string TableProfiles = $"{SchemaApplications}.[Profiles]";
        private const string TableProfileFormulas = $"{SchemaApplications}.[ProfileFormulas]";
        private const string TableProfileFormulaScores = $"{SchemaApplications}.[ProfileFormulaScores]";
        private const string TableExams = $"{SchemaApplications}.[Exams]";
        private const string TableSummaryExamResults = $"{SchemaApplications}.[SummaryExamResults]";

        private static void Probes()
        {

        }

        private static void Seed()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Seed");
            Console.ResetColor();

            var validityPeriodCount = 2; // 2
            var countryCount = 1; // 1
            var areaCount = 2 * countryCount; // 28 *
            var communityCount = 3 * areaCount; // 7 *
            var settlementCount = 4 * communityCount; // 9 *
            var regionCount = 2 * settlementCount; // 2 *
            var addressCount = 2 * regionCount; // 3 *
            var institutionCount = addressCount; // addressCount
            var professionDirectionWrapperCount = 4; // should be even 6 *
            var professionDirectionCount = 6 * professionDirectionWrapperCount; // should be even 8 *
            var professionCount = 6 * professionDirectionCount; // should be even 10 *
            var specialityCount = 8 * professionCount; // should be even 14 *
            var profilePerInstitutionCount = 5; // 5
            var formulaPerProfileCount = 2; // 2
            var subjectCount = 11; // 20
            var examCount = 2; // 2

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                InsertValidityPeriods(connection, validityPeriodCount);
                InsertCountries(connection, countryCount);
                InsertAreas(connection, areaCount);
                InsertCommunities(connection, communityCount);
                InsertLocations(connection, settlementCount + addressCount);
                InsertSettlements(connection, settlementCount);
                InsertRegions(connection, regionCount);
                InsertInstitutions(connection, institutionCount);
                InsertAddresses(connection, addressCount);
                InsertInstitutionsAddresses(connection);
                InsertWebsites(connection);
                InsertEmails(connection);
                InsertPhoneNumbers(connection);
                InsertProfessionDirectionWrappers(connection, professionDirectionWrapperCount);
                InsertProfessionDirections(connection, professionDirectionCount);
                InsertProfessions(connection, professionCount);
                InsertSpecialities(connection, specialityCount);
                InsertSubjects(connection, subjectCount);
                InsertProfiles(connection, profilePerInstitutionCount);
                InsertProfileFormulas(connection, formulaPerProfileCount);
                InsertProfileFormulaScores(connection);
                InsertExams(connection, examCount);
                InsertSummaryExamResults(connection);

                connection.Close();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Successful");
            Console.ResetColor();

            #region Help 1

            //var queryString =
            //    "SELECT ProductID, UnitPrice, ProductName from dbo.products" + " " +
            //    "WHERE UnitPrice > @pricePoint" + " " +
            //    "ORDER BY UnitPrice DESC;"
            //;

            //using (var connection = new SqlConnection("Server=.;Database=ApplicationInfoSys-SynonymId-to-Name;User=sa;Password=yourStrong(!)Password;Integrated Security=false;Encrypt=false;TrustServerCertificate=false;"))
            //{
            //    var command = new SqlCommand(queryString, connection);
            //    command.Parameters.AddWithValue("@pricePoint", 5);

            //    try
            //    {
            //        connection.Open();
            //        var reader = command.ExecuteReader();
            //        while (reader.Read())
            //        {
            //            Console.WriteLine("\t{0}\t{1}\t{2}", reader[0], reader[1], reader[2]);
            //        }

            //        reader.Close();
            //    }
            //    catch (Exception exception)
            //    {
            //        Console.WriteLine(exception.Message);
            //    }
            //}


            //          private void btnadd_Click(object sender, EventArgs e)
            //          {
            //              try
            //              {
            //                  //create  object  of Connection Class..................
            //                  SqlConnection con = new SqlConnection();

            //                  // Set Connection String property of Connection object..................
            //                  con.ConnectionString = "Data Source=KUSH-PC;Initial Catalog=test;Integrated           Security=True";

            //                  // Open Connection..................
            //                  con.Open();

            //                  //Create object of Command Class................
            //                  SqlCommand cmd = new SqlCommand();

            //                  //set Connection Property  of  Command object.............
            //                  cmd.Connection = con;
            //                  //Set Command type of command object
            //                  //1.StoredProcedure
            //                  //2.TableDirect
            //                  //3.Text   (By Default)

            //                  cmd.CommandType = CommandType.Text;

            //                  //Set Command text Property of command object.........

            //                  cmd.CommandText = "Insert into Registration (Username, password) values ('@user','@pass')";

            //                  //Assign values as `parameter`. It avoids `SQL Injection`
            //                  cmd.Parameters.AddWithValue("user", TextBox1.text);
            //                  cmd.Parameters.AddWithValue("pass", TextBox2.text);

            //                  Execute command by calling following method................1.ExecuteNonQuery()
            //                   This is used for insert, delete, update command...........
            //2.ExecuteScalar()
            //     This returns a single value.........(used only for select command)
            //                          3.ExecuteReader()
            //                 Return one or more than one record.

            //cmd.ExecuteNonQuery();
            //                  con.Close();


            //                  MessageBox.Show("Data Saved");
            //              }
            //              catch (Exception ex)
            //              {
            //                  MessageBox.Show(ex.Message);
            //                  con.Close();
            //              }


            //          }

            #endregion
        }

        private static int GetCount(SqlConnection connection, string table)
        {
            var command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = $"SELECT COUNT(*) FROM {table}";

            return (int)command.ExecuteScalar();
        }

        //[MethodImpl(MethodImplOptions.NoInlining)]
        private static void PrintCurrentMethodName()
        {
            Console.WriteLine(new StackTrace(new StackFrame(1))?.GetFrame(0)?.GetMethod()?.Name);
        }

        private static void InsertValidityPeriods(SqlConnection connection, int count)
        {
            PrintCurrentMethodName();

            var currentYear = 2023;

            for (int i = 0; i < count; i++)
            {
                var command = new SqlCommand();
                command.Connection = connection;

                command.CommandText =
                        $"  INSERT INTO {TableValidityPeriods} " +
                        $"  ( [From], [To]    )   VALUES " +
                        $"  ( @from,  @to     )"
                ;

                command.Parameters.AddWithValue("from", $"{(currentYear - i - 1)}-09-15 00:00:00.0000000");
                command.Parameters.AddWithValue("to", $"{(currentYear - i)}-09-14 23:59:59.9999999");

                command.ExecuteNonQuery();
            }
        }

        private static void InsertCountries(SqlConnection connection, int count)
        {
            PrintCurrentMethodName();

            for (int i = 0; i < count; i++)
            {
                var command = new SqlCommand();
                command.Connection = connection;

                command.CommandText =
                        $"  INSERT INTO {TableCountries} " +
                        $"  (   [Name], [PhoneCode] )   VALUES " +
                        $"  (   @name,  @phoneCode  )"
                ;

                command.Parameters.AddWithValue("name", $"Държава-{(1 + i)}");
                command.Parameters.AddWithValue("phoneCode", GetRandomPhoneCode());

                command.ExecuteNonQuery();
            }
        }

        private static void InsertAreas(SqlConnection connection, int count)
        {
            PrintCurrentMethodName();

            var countryCount = GetCount(connection, TableCountries);

            for (int i = 0; i < count; i++)
            {
                var command = new SqlCommand();
                command.Connection = connection;

                command.CommandText =
                        $"  INSERT INTO {TableAreas} " +
                        $"  (   [CountryId],    [Name]  )   VALUES " +
                        $"  (   @countryId,     @name   )"
                ;

                command.Parameters.AddWithValue("countryId", 1 + (i % countryCount));
                command.Parameters.AddWithValue("name", $"Област-{(1 + i)}");

                command.ExecuteNonQuery();
            }
        }

        private static void InsertCommunities(SqlConnection connection, int count)
        {
            PrintCurrentMethodName();

            var areaCount = GetCount(connection, TableAreas);

            for (int i = 0; i < count; i++)
            {
                var command = new SqlCommand();
                command.Connection = connection;

                command.CommandText =
                        $"  INSERT INTO {TableCommunities} " +
                        $"  (   [AreaId],   [Name]  )   VALUES " +
                        $"  (   @areaId,    @name   )"
                ;

                command.Parameters.AddWithValue("areaId", 1 + (i % areaCount));
                command.Parameters.AddWithValue("name", $"Община-{(1 + i)}");

                command.ExecuteNonQuery();
            }
        }

        private static void InsertLocations(SqlConnection connection, int count)
        {
            PrintCurrentMethodName();

            for (int i = 0; i < count; i++)
            {
                var command = new SqlCommand();
                command.Connection = connection;

                command.CommandText =
                        $"  INSERT INTO {TableLocations} " +
                        $"  (   [Latitude], [Longitude] )   VALUES " +
                        $"  (   @latitude,  @longitude  )"
                ;

                command.Parameters.AddWithValue("latitude", GetRandomLatitude());
                command.Parameters.AddWithValue("longitude", GetRandomLongitude());

                command.ExecuteNonQuery();
            }
        }

        private static void InsertSettlements(SqlConnection connection, int count)
        {
            PrintCurrentMethodName();

            var communityCount = GetCount(connection, TableCommunities);
            var ekattes = new HashSet<string>();

            for (int i = 0; i < count; i++)
            {
                var command = new SqlCommand();
                command.Connection = connection;

                command.CommandText =
                        $"  INSERT INTO {TableSettlements} " +
                        $"  (   [Ekatte],   [CommunityId],  [Type], [Name], [LocationId],   [PostalCode],   [PhoneCode] )   VALUES " +
                        $"  (   @ekatte,     @communityId,   @type,  @name,  @locationId,    @postalCode,   @phoneCode  )"
                ;

                do
                {
                    var ekatte = GetRandomEkatte();
                    if (ekattes.Add(ekatte))
                    {
                        command.Parameters.AddWithValue("ekatte", ekatte);
                        break;
                    }
                } while (true);

                command.Parameters.AddWithValue("communityId", 1 + (i % communityCount));
                command.Parameters.AddWithValue("type", GetRandomSettlementType(i));
                command.Parameters.AddWithValue("name", $"Селище-{(1 + i)}");
                command.Parameters.AddWithValue("locationId", 1 + i);
                command.Parameters.AddWithValue("postalCode", GetRandomPostalCode());
                command.Parameters.AddWithValue("phoneCode", GetRandomPhoneCode());

                command.ExecuteNonQuery();
            }
        }

        private static void InsertRegions(SqlConnection connection, int count)
        {
            PrintCurrentMethodName();

            var settlementCount = GetCount(connection, TableSettlements);

            for (int i = 0; i < count; i++)
            {
                var command = new SqlCommand();
                command.Connection = connection;

                command.CommandText =
                        $"  INSERT INTO {TableRegions} " +
                        $"  (   [SettlementId], [Name], [PostalCode]    )   VALUES " +
                        $"  (   @settlementId,  @name,  @postalCode     )"
                ;

                command.Parameters.AddWithValue("settlementId", 1 + (i % settlementCount));
                command.Parameters.AddWithValue("name", $"Регион-{(1 + i)}");
                command.Parameters.AddWithValue("postalCode", GetRandomPostalCode());

                command.ExecuteNonQuery();
            }
        }

        private static void InsertInstitutions(SqlConnection connection, int count)
        {
            PrintCurrentMethodName();

            var settlementCount = GetCount(connection, TableSettlements);
            var selfIds = new HashSet<int>();

            for (int i = 0; i < count; i++)
            {
                var command = new SqlCommand();
                command.Connection = connection;

                command.CommandText =
                        $"  INSERT INTO {TableInstitutions} " +
                        $"  (   [SelfId],   [IsClosed], [Status],   [TypeBy24to27Short],    [TypeBy35to36Ownership],    [TypeBy37Preparation],  [TypeBy38Detailed], [TypeBy39Specialized],  [TypeByFinancing],  [EIK],  [Name], [ShortName],    [Director], [ValidityPeriodId]  )   VALUES " +
                        $"  (   @selfId,    @isClosed,  @status,    @typeBy24to27Short,     @typeBy35to36Ownership,     @typeBy37Preparation,   @typeBy38Detailed,  @typeBy39Specialized,   @typeByFinancing,   @eIK,   @name,  @shortName,     @director,  @validityPeriodId   )"
                ;

                do
                {
                    var selfId = GetRandomInstitutionId();
                    if (selfIds.Add(selfId))
                    {
                        command.Parameters.AddWithValue("selfId", selfId);
                        break;
                    }
                } while (true);

                command.Parameters.AddWithValue("isClosed", i % 2 == 0);
                command.Parameters.AddWithValue("status", GetRandomInstitutionStatus(i));
                command.Parameters.AddWithValue("typeBy24to27Short", GetRandomInstitutionTypeBy24to27Short(i));
                command.Parameters.AddWithValue("typeBy35to36Ownership", GetRandomInstitutionTypeBy35to36Ownership(i));
                command.Parameters.AddWithValue("typeBy37Preparation", GetRandomInstitutionTypeBy37Preparation(i));
                command.Parameters.AddWithValue("typeBy38Detailed", GetRandomInstitutionTypeBy38Detailed(i));
                command.Parameters.AddWithValue("typeBy39Specialized", GetRandomInstitutionTypeBy39Specialized(i));
                command.Parameters.AddWithValue("typeByFinancing", GetRandomInstitutionTypeByFinancing(i));
                command.Parameters.AddWithValue("eIK", GetRandomInstitutionEik());
                command.Parameters.AddWithValue("name", $"Институция-{(1 + i)}");
                command.Parameters.AddWithValue("shortName", $"Инст.-{(1 + i)}");
                command.Parameters.AddWithValue("director", $"Директор-{(1 + i)}");
                command.Parameters.AddWithValue("validityPeriodId", 1);

                command.ExecuteNonQuery();
            }
        }

        private static void InsertAddresses(SqlConnection connection, int count)
        {
            PrintCurrentMethodName();

            var settlementCount = GetCount(connection, TableSettlements);
            var regionCount = GetCount(connection, TableRegions);

            for (int i = 0; i < count; i++)
            {
                var isSettlement = i % 3 != 0;

                var command = new SqlCommand();
                command.Connection = connection;

                command.CommandText =
                        $"  INSERT INTO {TableAddresses} " +
                        $"  (   [{(isSettlement ? "SettlementId" : "RegionId")}],   [Name], [LocationId],   [PostalCode]    )   VALUES " +
                        $"  (   @settlementIdOrRegionId,                            @name,  @locationId,    @postalCode     )"
                ;

                command.Parameters.AddWithValue("settlementIdOrRegionId", 1 + (i % (isSettlement ? settlementCount : regionCount)));
                command.Parameters.AddWithValue("name", $"Адрес-{(1 + i)}-Квартал-Улица-Номер-Блок-Вход-Етаж");
                command.Parameters.AddWithValue("locationId", 1 + i);
                command.Parameters.AddWithValue("postalCode", GetRandomPostalCode());

                command.ExecuteNonQuery();
            }
        }

        private static void InsertInstitutionsAddresses(SqlConnection connection)
        {
            PrintCurrentMethodName();

            var institutionCount = GetCount(connection, TableInstitutions);

            for (int i = 0; i < institutionCount; i++)
            {
                var isAdministrative = i % 8 != 0;

                var command = new SqlCommand();
                command.Connection = connection;

                command.CommandText =
                        $"  INSERT INTO {TableInstitutionsAddresses} " +
                        $"  (   [InstitutionId],    [AddressId],    [IsAdministrative], [IsActivity]    )   VALUES " +
                        $"  (   @institutionId,     @addressId,     @isAdministrative,  @isActivity     )"
                ;

                command.Parameters.AddWithValue("institutionId", 1 + i);
                command.Parameters.AddWithValue("addressId", 1 + (i % 7 != 0 ? i : (i < 1 ? 0 : i - 1)));
                command.Parameters.AddWithValue("isAdministrative", isAdministrative ? 1 : 0);
                command.Parameters.AddWithValue("isActivity", (!isAdministrative ? true : i % 9 != 0) ? 1 : 0);

                command.ExecuteNonQuery();
            }
        }

        private static void InsertWebsites(SqlConnection connection)
        {
            PrintCurrentMethodName();

            var institutionCount = GetCount(connection, TableInstitutions);

            for (int i = 0; i < institutionCount; i++)
            {
                var command = new SqlCommand();
                command.Connection = connection;

                command.CommandText =
                        $"  INSERT INTO {TableWebsites} " +
                        $"  (   [Address],  [InstitutionId] )   VALUES " +
                        $"  (   @address1,  @institutionId  )" +
                    (
                        i % 5 != 0 ?
                        string.Empty :
                        $", (   @address2,  @institutionId  )"
                    )
                ;

                command.Parameters.AddWithValue("address1", $"Website-{(1 + i)}-1.com");
                command.Parameters.AddWithValue("address2", $"Website-{(1 + i)}-2.org");
                command.Parameters.AddWithValue("institutionId", 1 + i);

                command.ExecuteNonQuery();
            }
        }

        private static void InsertEmails(SqlConnection connection)
        {
            PrintCurrentMethodName();

            var institutionCount = GetCount(connection, TableInstitutions);

            for (int i = 0; i < institutionCount; i++)
            {
                var command = new SqlCommand();
                command.Connection = connection;

                command.CommandText =
                        $"  INSERT INTO {TableEmails} " +
                        $"  (   [Address],  [InstitutionId] )   VALUES " +
                        $"  (   @address1,  @institutionId  )" +
                    (
                        i % 6 != 0 ?
                        string.Empty :
                        $", (   @address2,  @institutionId  )"
                    )
                ;

                command.Parameters.AddWithValue("address1", $"Email-1@Website-X-{(1 + i)}.com");
                command.Parameters.AddWithValue("address2", $"Email-2@Website-X-{(1 + i)}.org");
                command.Parameters.AddWithValue("institutionId", 1 + i);

                command.ExecuteNonQuery();
            }
        }

        private static void InsertPhoneNumbers(SqlConnection connection)
        {
            PrintCurrentMethodName();

            var countryCount = GetCount(connection, TableCountries);
            var settlementCount = GetCount(connection, TableSettlements);
            var institutionCount = GetCount(connection, TableInstitutions);

            for (int i = 0; i < institutionCount; i++)
            {
                var command = new SqlCommand();
                command.Connection = connection;

                command.CommandText =
                        $"  INSERT INTO {TablePhoneNumbers} " +
                        $"  (   [CountryId],    [SettlementId], [Number],   [Additional],   [InstitutionId] )   VALUES " +
                        $"  (   @countryId1,    @settlementId1, @number1,   @additional1,   @institutionId  )" +
                    (
                        i % 6 != 0 ?
                        string.Empty :
                        $", (   @countryId2,    @settlementId2, @number2,   @additional2,   @institutionId  )"
                    )
                ;

                command.Parameters.AddWithValue("countryId1", 1 + (i % countryCount));
                command.Parameters.AddWithValue("countryId2", DBNull.Value);
                command.Parameters.AddWithValue("settlementId1", DBNull.Value);
                command.Parameters.AddWithValue("settlementId2", 1 + (i % settlementCount));
                command.Parameters.AddWithValue("number1", GetRandomMobilePhoneNumber());
                command.Parameters.AddWithValue("number2", GetRandomLandlinePhoneNumber());
                command.Parameters.AddWithValue("additional1", $"Допълнително-{(1 + i)}-1");
                command.Parameters.AddWithValue("additional2", $"Допълнително-{(1 + i)}-2");
                command.Parameters.AddWithValue("institutionId", 1 + i);

                command.ExecuteNonQuery();
            }
        }

        private static void InsertProfessionDirectionWrappers(SqlConnection connection, int count)
        {
            PrintCurrentMethodName();

            for (int i = 0; i < count; i++)
            {
                var command = new SqlCommand();
                command.Connection = connection;

                command.CommandText =
                        $"  INSERT INTO {TableProfessionDirectionWrappers} " +
                        $"  (   [SelfId],   [Name], [IsDroppedOut], [Description]   )   VALUES " +
                        $"  (   @selfId,    @name,  @isDroppedOut,  @description    )"
                ;

                command.Parameters.AddWithValue("selfId", 1 + (i % (count / 2) != 0 ? i : (i < 1 ? 0 : i - 1)));
                command.Parameters.AddWithValue("name", $"Над професионално направление-{(1 + (i % count != 0 ? i : (i < 1 ? 0 : i - 1)))}");
                command.Parameters.AddWithValue("isDroppedOut", 0);
                command.Parameters.AddWithValue("description", $"Описание-{(1 + i)}");

                command.ExecuteNonQuery();
            }
        }

        private static void InsertProfessionDirections(SqlConnection connection, int count)
        {
            PrintCurrentMethodName();

            var professionDirectionWrapperCount = GetCount(connection, TableProfessionDirectionWrappers);

            for (int i = 0; i < count; i++)
            {
                var command = new SqlCommand();
                command.Connection = connection;

                command.CommandText =
                        $"  INSERT INTO {TableProfessionDirections} " +
                        $"  (   [ProfessionDirectionWrapperId], [SelfId],   [Name], [IsDroppedOut], [Description]   )   VALUES " +
                        $"  (   @professionDirectionWrapperId,  @selfId,    @name,  @isDroppedOut,  @description    )"
                ;

                command.Parameters.AddWithValue("professionDirectionWrapperId", 1 + (i % professionDirectionWrapperCount));
                command.Parameters.AddWithValue("selfId", 1 + (i % (count / 2) != 0 ? i : (i < 1 ? 0 : i - 1)));
                command.Parameters.AddWithValue("name", $"Професионално направление-{(1 + (i % count != 0 ? i : (i < 1 ? 0 : i - 1)))}");
                command.Parameters.AddWithValue("isDroppedOut", 0);
                command.Parameters.AddWithValue("description", $"Описание-{(1 + i)}");

                command.ExecuteNonQuery();
            }
        }

        private static void InsertProfessions(SqlConnection connection, int count)
        {
            PrintCurrentMethodName();

            var professionDirectionCount = GetCount(connection, TableProfessionDirections);

            for (int i = 0; i < count; i++)
            {
                var command = new SqlCommand();
                command.Connection = connection;

                command.CommandText =
                        $"  INSERT INTO {TableProfessions} " +
                        $"  (   [ProfessionDirectionId],    [SelfId],   [Name], [IsDroppedOut], [Description]   )   VALUES " +
                        $"  (   @professionDirectionId,     @selfId,    @name,  @isDroppedOut,  @description    )"
                ;

                command.Parameters.AddWithValue("professionDirectionId", 1 + (i % professionDirectionCount));
                command.Parameters.AddWithValue("selfId", 1 + (i % (count / 2) != 0 ? i : (i < 1 ? 0 : i - 1)));
                command.Parameters.AddWithValue("name", $"Професия-{(1 + (i % count != 0 ? i : (i < 1 ? 0 : i - 1)))}");
                command.Parameters.AddWithValue("isDroppedOut", 0);
                command.Parameters.AddWithValue("description", $"Описание-{(1 + i)}");

                command.ExecuteNonQuery();
            }
        }

        private static void InsertSpecialities(SqlConnection connection, int count)
        {
            PrintCurrentMethodName();

            var professionCount = GetCount(connection, TableProfessions);

            for (int i = 0; i < count; i++)
            {
                var command = new SqlCommand();
                command.Connection = connection;

                command.CommandText =
                        $"  INSERT INTO {TableSpecialities} " +
                        $"  (   [ProfessionId], [SelfId],   [Name], [ProfessionalQualificationDegree],  [IsDroppedOut], [IsProtected],  [IsWithExpectedShortage],   [Description]   )   VALUES " +
                        $"  (   @professionId,  @selfId,    @name,  @professionalQualificationDegree,   @isDroppedOut,  @isProtected,   @isWithExpectedShortage,    @description    )"
                ;

                command.Parameters.AddWithValue("professionId", 1 + (i % professionCount));
                command.Parameters.AddWithValue("selfId", 1 + (i % (count / 2) != 0 ? i : (i < 1 ? 0 : i - 1)));
                command.Parameters.AddWithValue("name", $"Специалност-{(1 + (i % count != 0 ? i : (i < 1 ? 0 : i - 1)))}");
                command.Parameters.AddWithValue("professionalQualificationDegree", 1 + (i % 4));
                command.Parameters.AddWithValue("isDroppedOut", i % 9 == 0 ? 1 : 0);
                command.Parameters.AddWithValue("isProtected", i % 11 == 0 ? 1 : 0);
                command.Parameters.AddWithValue("isWithExpectedShortage", i % 13 == 0 ? 1 : 0);
                command.Parameters.AddWithValue("description", $"Описание-{(1 + i)}");

                command.ExecuteNonQuery();
            }
        }

        private static void InsertSubjects(SqlConnection connection, int count)
        {
            PrintCurrentMethodName();

            for (int i = 0; i < count; i++)
            {
                var command = new SqlCommand();
                command.Connection = connection;

                command.CommandText =
                        $"  INSERT INTO {TableSubjects} " +
                        $"  (   [Name], [ShortName] )   VALUES " +
                        $"  (   @name,  @shortName  )"
                ;

                command.Parameters.AddWithValue("name", $"Предмет-{(1 + i)}");
                command.Parameters.AddWithValue("shortName", $"Пм-{(1 + i)}");

                command.ExecuteNonQuery();
            }
        }

        private static void InsertProfiles(SqlConnection connection, int countPerInstitution)
        {
            PrintCurrentMethodName();

            var institutionCount = GetCount(connection, TableInstitutions);
            var specialityCount = GetCount(connection, TableSpecialities);
            var subjectCount = GetCount(connection, TableSubjects);
            var selfId = 0;

            for (int i = 0; i < institutionCount; i++)
            {
                for (int c = 0; c < countPerInstitution; c++)
                {
                    selfId++;
                    var unitSeats = 2 * 13;
                    var count = Random.Next(1, 3);
                    var isByQuotas = (2 * i + 3 * c) % 5 == 0;
                    var totalSeats = isByQuotas ? 0 : unitSeats * count;
                    var menSeats = !isByQuotas ? 0 : count * (unitSeats / 2);
                    var womenSeats = !isByQuotas ? 0 : count * (unitSeats / 2);

                    var command = new SqlCommand();
                    command.Connection = connection;

                    command.CommandText =
                            $"  INSERT INTO {TableProfiles} " +
                            $"  (   [InstitutionId],    [SelfId],   [Name], [Type], [Form], [SpecialityId], [SubjectId],    [StudyingWay],  [Requester],    [Grade],    [Duration], [IsNewForInstitution],  [IsNewForCommunity],    [IsNewForArea], [Count],    [IsByQuotas],   [TotalSeats],   [MenSeats], [WomenSeats]    )   VALUES " +
                            $"  (   @institutionId,     @selfId,    @name,  @type,  @form,  @specialityId,  @subjectId,     @studyingWay,   @requester,     @grade,     @duration,  @isNewForInstitution,   @isNewForCommunity,     @isNewForArea,  @count,     @isByQuotas,    @totalSeats,    @menSeats,  @womenSeats     )"
                    ;

                    command.Parameters.AddWithValue("institutionId", 1 + i);
                    command.Parameters.AddWithValue("selfId", selfId);
                    command.Parameters.AddWithValue("name", $"Паралелка-{selfId}-Езици");
                    command.Parameters.AddWithValue("type", GetRandomProfileType());
                    command.Parameters.AddWithValue("form", GetRandomProfileForm());
                    command.Parameters.AddWithValue("specialityId", 1 + ((3 * i + 4 * c) % specialityCount));
                    command.Parameters.AddWithValue("subjectId", 1 + ((10 * i + 11 * c) % subjectCount));
                    command.Parameters.AddWithValue("studyingWay", GetRandomProfileLanguageStudyingWay());
                    command.Parameters.AddWithValue("requester", $"Заявител-{1 + ((4 * i + 5 * c) / 10)}");
                    command.Parameters.AddWithValue("grade", GetRandomGrade((5 * i + 6 * c) % Grades.Count));
                    command.Parameters.AddWithValue("duration", 3 + ((7 * i + 8 * c) % 10 == 0 ? 0 : 2));
                    command.Parameters.AddWithValue("isNewForInstitution", (8 * i + 9 * c) % 100 == 0 ? 1 : 0);
                    command.Parameters.AddWithValue("isNewForCommunity", (8 * i + 9 * c) % 200 == 0 ? 1 : 0);
                    command.Parameters.AddWithValue("isNewForArea", (8 * i + 9 * c) % 400 == 0 ? 1 : 0);
                    command.Parameters.AddWithValue("count", count);
                    command.Parameters.AddWithValue("isByQuotas", isByQuotas ? 1 : 0);
                    command.Parameters.AddWithValue("totalSeats", totalSeats);
                    command.Parameters.AddWithValue("menSeats", menSeats);
                    command.Parameters.AddWithValue("womenSeats", womenSeats);

                    command.ExecuteNonQuery();
                }
            }
        }

        private static void InsertProfileFormulas(SqlConnection connection, int formulaPerProfileCount)
        {
            PrintCurrentMethodName();

            var profileCount = GetCount(connection, TableProfiles);

            for (int p = 0; p < profileCount; p++)
            {
                for (int f = 0; f < formulaPerProfileCount; f++)
                {
                    var command = new SqlCommand();
                    command.Connection = connection;

                    command.CommandText =
                            $"  INSERT INTO {TableProfileFormulas} " +
                            $"  (   [ProfileId],    [Formula]   )   VALUES " +
                            $"  (   @profileId,     @formula    )"
                    ;

                    command.Parameters.AddWithValue("profileId", $"{1 + p}");
                    command.Parameters.AddWithValue("formula", $"Балообразуваща-формула-{1 + f}");

                    command.ExecuteNonQuery();
                }
            }
        }

        private static void InsertProfileFormulaScores(SqlConnection connection)
        {
            PrintCurrentMethodName();

            var profileCount = GetCount(connection, TableProfiles);

            for (int s = 0; s < 4; s++)
            {
                for (int p = 0; p < profileCount; p++)
                {
                    decimal minMen = Random.Next(0, 400);
                    decimal minWomen = Random.Next(0, 400);
                    decimal maxMen = Random.Next(1 + (int)minMen, 501);
                    decimal maxWomen = Random.Next(1 + (int)minWomen, 501);

                    var command = new SqlCommand();
                    command.Connection = connection;

                    command.CommandText =
                            $"  INSERT INTO {TableProfileFormulaScores} " +
                            $"  (   [Stage],    [ProfileId],    [MinTotal], [MinMen],   [MinWomen], [MaxTotal], [MaxMen],   [MaxWomen]  )   VALUES " +
                            $"  (   @stage,     @profileId,     @minTotal,  @minMen,    @minWomen,  @maxTotal,  @maxMen,    @maxWomen   )"
                    ;

                    command.Parameters.AddWithValue("stage", 1 + s);
                    command.Parameters.AddWithValue("profileId", 1 + p);
                    command.Parameters.AddWithValue("minTotal", Math.Min(minMen, minWomen));
                    command.Parameters.AddWithValue("minMen", minMen);
                    command.Parameters.AddWithValue("minWomen", minWomen);
                    command.Parameters.AddWithValue("maxTotal", Math.Max(maxMen, maxWomen));
                    command.Parameters.AddWithValue("maxMen", maxMen);
                    command.Parameters.AddWithValue("maxWomen", maxWomen);

                    command.ExecuteNonQuery();
                }
            }
        }

        private static void InsertExams(SqlConnection connection, int count)
        {
            PrintCurrentMethodName();

            for (int i = 0; i < count; i++)
            {
                var command = new SqlCommand();
                command.Connection = connection;

                command.CommandText =
                        $"  INSERT INTO {TableExams} " +
                        $"  (   [Name]  )   VALUES " +
                        $"  (   @name   )"
                ;

                command.Parameters.AddWithValue("name", $"Exam-{(1 + i)}");

                command.ExecuteNonQuery();
            }
        }

        private static void InsertSummaryExamResults(SqlConnection connection)
        {
            PrintCurrentMethodName();

            var examCount = GetCount(connection, TableExams);
            var institutionCount = GetCount(connection, TableInstitutions);
            var subjectCount = GetCount(connection, TableSubjects);

            for (int e = 0; e < examCount; e++)
            {
                for (int g = 0; g < Grades.Count; g++)
                {
                    for (int i = 0; i < institutionCount; i++)
                    {
                        for (int s = 0; s < subjectCount; s++)
                        {
                            var takers = Random.Next(0, 200);

                            var command = new SqlCommand();
                            command.Connection = connection;

                            command.CommandText =
                                    $"  INSERT INTO {TableSummaryExamResults} " +
                                    $"  (   [ExamId],   [Grade],    [InstitutionId],    [SubjectId],    [PreparationStyle], [PreparationType],  [Takers],   [AverageSuccess]    )   VALUES " +
                                    $"  (   @examId,    @grade,     @institutionId,     @subjectId,     @preparationStyle,  @preparationType,   @takers,    @averageSuccess     )"
                            ;

                            command.Parameters.AddWithValue("examId", 1 + e);
                            command.Parameters.AddWithValue("grade", GetRandomGrade(g));
                            command.Parameters.AddWithValue("institutionId", 1 + i);
                            command.Parameters.AddWithValue("subjectId", 1 + s);
                            command.Parameters.AddWithValue("preparationStyle", GetRandomExamPreparationStyle(i));
                            command.Parameters.AddWithValue("preparationType", GetRandomExamPreparationType(i));
                            command.Parameters.AddWithValue("takers", takers);
                            command.Parameters.AddWithValue("averageSuccess", GetRandomAverageSuccess(takers));

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private static Random Random = new Random();

        private static decimal GetRandomCoordinate(int lowerLimit, int upperLimit, int precision = 15)
        {
            var latitude = (decimal)(Random.Next(lowerLimit, upperLimit + 1) + Random.NextDouble());
            latitude = latitude <= upperLimit ? latitude : upperLimit;

            var latitudeAsString = latitude.ToString();
            var indexOfDot = latitudeAsString.IndexOf(".");
            if (
                    indexOfDot != -1 &&
                    precision < latitudeAsString.Length - indexOfDot - 1
                )
            {
                latitudeAsString = latitudeAsString.Substring(0, indexOfDot + 1 + precision);
            }

            latitude = decimal.Parse(latitudeAsString);

            return latitude;
        }

        private static decimal GetRandomLatitude() => GetRandomCoordinate(-90, 90, 15);
        private static decimal GetRandomLongitude() => GetRandomCoordinate(-180, 180, 15);

        private static string GetRandomPhoneCode() => Random.Next(1, 10_000).ToString();
        private static string GetRandomPostalCode() => Random.Next(1, 10_000).ToString();

        private static string GetRandomDigitsAsString(int min, int max, bool canBeginsWith0 = false)
        {
            if (
                    min < 0 ||
                    max < 0 ||
                    max < min
                )
            {
                throw new ArgumentException("Incorrect arguments: min, max");
            }

            var length = Random.Next(min, max + 1);
            if (length == 0)
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(Random.Next(0 < i ? 0 : (canBeginsWith0 ? 0 : 1), 10));
            }

            return stringBuilder.ToString();
        }

        private static string GetRandomEkatte() => GetRandomDigitsAsString(5, 5, true);

        private static T? GetElement<T>(IEnumerable<T> enumerable, int? atIndex = null)
        {
            if (enumerable == null)
            {
                throw new ArgumentException($"Enumeration is null");
            }

            if (!enumerable.Any())
            {
                throw new ArgumentException($"Enumeration has no elements");
            }

            var enumerableCount = enumerable.Count();
            var targetIndex = atIndex == null ? Random.Next(0, enumerableCount) : atIndex % enumerableCount;
            var targetElement = default(T);

            if (
                    targetIndex < 0 ||
                    targetIndex >= enumerableCount
                )
            {
                throw new ArgumentOutOfRangeException($"atIndex = {atIndex} is out of range [{0}, {(enumerableCount - 1)}]");
            }

            var counter = -1;
            foreach (var element in enumerable)
            {
                counter++;
                if (counter == targetIndex)
                {
                    targetElement = element;
                    break;
                }
            }

            return targetElement;
        }

        private static string? GetRandomSettlementType(int? atIndex = null) => GetElement(new List<string>() { "град", "село", }, atIndex);

        private static int GetRandomInstitutionId() => Random.Next(100_000, 1_000_000);
        private static string? GetRandomInstitutionStatus(int? atIndex = null) => GetElement(new List<string>() { "действаща", "действаща (не провежда учебен процес през текущата година)", "отписана", }, atIndex);
        private static string? GetRandomInstitutionTypeBy24to27Short(int? atIndex = null) => GetElement(new List<string>() { "училище", "детска градина", }, atIndex);
        private static string? GetRandomInstitutionTypeBy35to36Ownership(int? atIndex = null) => GetElement(new List<string>() { "общинско", "държавно", "частно", "духовно", "по силата на международен договор", }, atIndex);
        private static string? GetRandomInstitutionTypeBy37Preparation(int? atIndex = null) => GetElement(new List<string>() { "специализирано", "неспециализирано", }, atIndex);
        private static string? GetRandomInstitutionTypeBy38Detailed(int? atIndex = null) => GetElement(new List<string>() { "професионална гимназия", "профилирана гимназия", "основно училище", }, atIndex);
        private static string? GetRandomInstitutionTypeBy39Specialized(int? atIndex = null) => GetElement(new List<string>() { "по изкуствата", "по културата", "спортно", "духовно", }, atIndex);
        private static string? GetRandomInstitutionTypeByFinancing(int? atIndex = null) => GetElement(new List<string>() { "МОН", "община", "частно", }, atIndex);
        private static string? GetRandomInstitutionEik() => GetRandomDigitsAsString(9, 15, true);

        private static string? GetRandomMobilePhoneNumber() => GetRandomDigitsAsString(7, 8, false);
        private static string? GetRandomLandlinePhoneNumber() => GetRandomDigitsAsString(9, 9, false);

        private static string? GetRandomProfileType(int? atIndex = null) => GetElement(new List<string>() { "професионална", "профилирана", }, atIndex);
        private static string? GetRandomProfileForm(int? atIndex = null) => GetElement(new List<string>() { "дневна", "дуална", "индивидуална", }, atIndex);
        private static string? GetRandomProfileLanguageStudyingWay(int? atIndex = null) => GetElement(new List<string>() { "разширено", "интензивно", "нито разширено, нито интензивно", }, atIndex);

        private static string? GetRandomExamName(int? atIndex = null) => GetElement(new List<string>() { "национално външно оценяване", "държавен зрелостен изпит", }, atIndex);

        private static string? GetRandomExamPreparationStyle(int? atIndex = null) => GetElement(new List<string>() { "общообразователна подготовка", "профилирана подготовка", }, atIndex);
        private static string? GetRandomExamPreparationType(int? atIndex = null) => GetElement(new List<string>() { "задължителна", "избираема", }, atIndex);

        private static List<int> Grades = new List<int>() { 4, 7, 10, 12, };
        private static int? GetRandomGrade(int? atIndex = null) => GetElement(Grades, atIndex);
        private static decimal GetRandomAverageSuccess(int takers)
        {
            if (takers < 1)
            {
                return 0;
            }

            var result = (decimal)Random.Next(0, 100);
            result = decimal.Parse(Random.Next(0, 100 + (result == 0 ? 1 : 0)) + "." + result);

            return result;
        }
    }
}
