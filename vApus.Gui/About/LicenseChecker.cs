using IntelliLock.Licensing;
using RandomUtils.Log;
using System;
using System.IO;
using System.Windows.Forms;

namespace vApus.Gui {
    public static class LicenseChecker {
        public static event EventHandler LicenseCheckFinished;

        private static readonly string[] LicenseStatusses = {
            "License not checked", "Licensed", "Evaluation mode", "Evaluation expired", "License file not found", "Hardware did not match the license",
            "Invalid signature", "Server validation failed", "Deactivated", "Reactivated", "Floating license users exceeded",
            "Floating license server error", "Full version expired", "Floating license server timeout"
        };

        public static string Status { get; private set; }

        static LicenseChecker() {
            EvaluationMonitor.LicenseCheckFinished += EvaluationMonitor_LicenseCheckFinished;
            CheckLicense();
        }

        public static void CheckLicense() {
            Status = "Checking license...";
            string license = Path.Combine(Application.StartupPath, "license.license");
            if (File.Exists(license))
                EvaluationMonitor.LoadLicense(license);
            else
                Status = "No license file found.";
        }

        private static void EvaluationMonitor_LicenseCheckFinished() {
            try {
                Status = "License status: ";

                if (EvaluationMonitor.CurrentLicense.ExpirationDate_Enabled) {
                    if (EvaluationMonitor.CurrentLicense.ExpirationDate < DateTime.Now)
                        Status += "Expired\n\n";
                    else
                        Status += LicenseStatusses[(int)EvaluationMonitor.CurrentLicense.LicenseStatus] + "\n\n";

                    Status += "License valid until " + EvaluationMonitor.CurrentLicense.ExpirationDate + "\n\n";
                }
                else {
                    Status += LicenseStatusses[(int)EvaluationMonitor.CurrentLicense.LicenseStatus] + "\n\n";
                }

                if (EvaluationMonitor.CurrentLicense.LicenseInformation.Count != 0) {
                    Status += "Licensed to\n";
                    for (int i = 0; i != EvaluationMonitor.CurrentLicense.LicenseInformation.Count; i++)
                        Status += "  " + EvaluationMonitor.CurrentLicense.LicenseInformation.GetByIndex(i) + "\n";
                }
            }
            catch (Exception ex) {
                Status = "Checking license failed!";
                Loggers.Log(Level.Error, Status, ex);
            }

            if (LicenseCheckFinished != null) LicenseCheckFinished(null, new EventArgs());
        }

    }
}
