namespace FakeGPS
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;
    using CommandLine;
    using FakeGPS.Common;

    /// <summary>
    /// The FakeGPS Command Line Program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The Command Line Arguments.</param>
        public static void Main(string[] args)
        {
            // If no arguments, open the GUI (Windows Form)
            if (args == null || args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FakeGPSForm());
                return;
            }

            var parser = new Parser();

            // get the command line parser results
            var results = parser.ParseArguments<Options>(args);

            // execute program functionality and return an exit code
            var exitCode = 1;

            results.WithParsed(
                options =>
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(options.LatLong))
                        {
                            // we have been given a set option
                            if (!GeolocationHelper.IsValid(options.LatLong))
                            {
                                ErrorHelper.InvalidArguments("Invalid LatLong");
                            }

                            // set the value to the registry
                            var latLong = GeolocationHelper.ToLatLong(options.LatLong);
                            RegistryHelper.SetLatLong(latLong);

                            Console.WriteLine("The following location has been set in the driver's registry settings:");
                            ConsoleHelper.WriteLatLong(latLong);
                        }

                        if (options.Get)
                        {
                            // get the value from the location API
                            var latLong = GeolocationHelper.Get();

                            Console.WriteLine("The following location has been returned from the Windows location API:");
                            ConsoleHelper.WriteLatLong(latLong);
                        }
                    }
                    catch (Exception ex)
                    {
                        ConsoleHelper.WriteError(ex.Message);

                        // if the debugger is attached, break.
                        if (Debugger.IsAttached)
                        {
                            Debugger.Break();
                        }

                        exitCode = 1;
                    }

                    exitCode = 0;
                });

            ConsoleHelper.WriteDebugExit();
            Console.ReadKey();
            Environment.Exit(exitCode);
        }
    }
}
