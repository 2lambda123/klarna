﻿namespace Klarna.Common.Configuration
{
    public class PaymentsConfiguration : ConnectionConfiguration
    {
        public string Mid { get; set; }
        public bool CustomerPreAssessment { get; set; }
        public bool SendProductAndImageUrl { get; set; } = true;
        public bool UseAttachments { get; set; }
        public bool AutoCapture { get; set; }
        public string WidgetBorderRadius { get; set; }
        public string WidgetDetailsColor { get; set; }
        public string WidgetBorderColor { get; set; }
        public string WidgetSelectedBorderColor { get; set; }
        public string WidgetTextColor { get; set; }
        public string ConfirmationUrl { get; set; }
        public string NotificationUrl { get; set; }
        public string PushUrl { get; set; }
        public string Design { get; set; }
    }
}
