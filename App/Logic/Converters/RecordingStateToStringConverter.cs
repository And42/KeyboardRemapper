using System;
using System.Globalization;
using MVVM_Tools.Code.Classes;

namespace App.Logic.Converters
{
    public class RecordingStateToStringConverter : ConverterBase<RecordingStates, string>
    {
        public override string ConvertInternal(RecordingStates value, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case RecordingStates.Idle:
                    return "Recording stopped";
                case RecordingStates.SourceKey:
                    return "Recording source key";
                case RecordingStates.MappedKey:
                    return "Recording mapped key";
                default:
                    throw new Exception("unknown recording state");
            }
        }
    }
}
