﻿using Microsoft.Win32.TaskScheduler;
using System.ServiceProcess;

namespace LenovoLegionToolkit.Lib
{
    public class VantageController
    {
        private static readonly string[] scheduledTasksPaths = new[]
        {
            "Lenovo\\BatteryGauge",
            "Lenovo\\ImController",
            "Lenovo\\ImController\\Plugins",
            "Lenovo\\ImController\\TimeBasedEvents",
            "Lenovo\\Vantage",
            "Lenovo\\Vantage\\Schedule",
        };

        private static readonly string[] serviceNames = new[]
        {
            "ImControllerService",
            "LenovoVantageService",
        };

        public void Enable()
        {
            SetScheduledTasksEnabled(true);
            SetServicesEnabled(true);
        }

        public void Disable()
        {
            SetScheduledTasksEnabled(false);
            SetServicesEnabled(false);
        }

        private void SetScheduledTasksEnabled(bool enabled)
        {
            var taskService = TaskService.Instance;
            foreach (var path in scheduledTasksPaths)
                SetTasksInFolderEnabled(taskService, path, enabled);
        }

        private void SetTasksInFolderEnabled(TaskService taskService, string path, bool enabled)
        {
            var folder = taskService.GetFolder(path);
            if (folder is null)
                return;

            foreach (var task in folder.Tasks)
            {
                task.Definition.Settings.Enabled = enabled;
                task.RegisterChanges();
            }
        }

        private void SetServicesEnabled(bool enabled)
        {
            foreach (var serviceName in serviceNames)
                SetServiceEnabled(serviceName, enabled);
        }

        private void SetServiceEnabled(string serviceName, bool enabled)
        {
            var service = new ServiceController(serviceName);

            try
            {
                var startMode = enabled ? ServiceStartMode.Automatic : ServiceStartMode.Disabled;
                service.ChangeStartMode(startMode);

                if (enabled)
                {
                    if (service.Status != ServiceControllerStatus.Running)
                        service.Start();
                }
                else
                {
                    if (service.CanStop)
                        service.Stop();
                }
            }
            finally
            {
                service.Close();
            }
        }
    }
}
