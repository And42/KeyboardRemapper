using System;
using System.Globalization;
using App.ViewModels;
using MVVM_Tools.Code.Classes;

namespace App.Logic.Converters
{
    public class RecordingStateToStringConverter : ConverterBase<NewMappingWindowViewModel.RecordingStates, string>
    {
        public override string ConvertInternal(NewMappingWindowViewModel.RecordingStates value, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case NewMappingWindowViewModel.RecordingStates.Idle:
                    return "Recording stopped";
                case NewMappingWindowViewModel.RecordingStates.SourceKey:
                    return "Recording source key";
                case NewMappingWindowViewModel.RecordingStates.MappedKey:
                    return "Recording mapped key";
                default:
                    throw new Exception("unknown recording state");
            }
        }
    }
}
