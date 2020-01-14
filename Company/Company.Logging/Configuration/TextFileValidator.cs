using Company.Logging.Contracts;
using System;

namespace Company.Logging.Configuration
{
  public class TextFileValidator : IListenerValidator
  {
    public void Validate(LogListener listener)
    {
      VerifyTextAppendParameter(listener);
      VerifyTextMaxSizeParameter(listener);
      VerifyPreserveLogFileNameExtensionParameter(listener);
      VerifyCountDirectionParameter(listener);
      VerifyTextRollBackBackupsParameter(listener);
      VerifyFileNameAndFilePatternParameters(listener);
    }

    private void VerifyCountDirectionParameter(LogListener listener)
    {
      ValidateInteger(listener, TextFileConstants.CountDirection);
    }

    private void VerifyPreserveLogFileNameExtensionParameter(LogListener listener)
    {
      ValidateBoolean(listener, TextFileConstants.PreserveLogFileNameExtension);
    }

    private void VerifyFileNameAndFilePatternParameters(LogListener listener)
    {
      string fileName = listener.Params.ContainsKey(TextFileConstants.FileName) ? listener.Params[TextFileConstants.FileName] : string.Empty;
      string filePatternName = listener.Params.ContainsKey(TextFileConstants.FilePatternName)
                                 ? listener.Params[TextFileConstants.FilePatternName] : string.Empty;
      if (string.IsNullOrEmpty(fileName.Trim()) && 
        string.IsNullOrEmpty(filePatternName.Trim())) {

        throw new InvalidOperationException("parameters fileName and filePatternName are empty");
      }
      if (!string.IsNullOrEmpty(fileName.Trim()) && 
        !string.IsNullOrEmpty(filePatternName.Trim())) {

        throw new InvalidOperationException("one of the parameters fileName and filePatternName must be empty");
      }
    }

    private void VerifyTextRollBackBackupsParameter(LogListener listener)
    {
      ValidateInteger(listener, TextFileConstants.RollBackBackups);
    }

    private void VerifyTextAppendParameter(LogListener listener)
    {
      string append = listener.Params[TextFileConstants.Append];
      if (append != TextFileConstants.RolloverValue && append != TextFileConstants.AppendValue)
      {
        throw new InvalidOperationException("append parameter has invalid value");
      }
    }

    private void VerifyTextMaxSizeParameter(LogListener listener)
    {
      ValidateInteger(listener, TextFileConstants.MaxSize);
    }

    private void ValidateInteger(LogListener listener, string name)
    {
      if (listener.Params.ContainsKey(name))
      {
        string maxSizeStr = listener.Params[name];
        int maxSize;
        if (!int.TryParse(maxSizeStr, out maxSize) || maxSize <= 0)
        {
          throw new InvalidOperationException(string.Format("{0} parameter has invalid value", name));
        }
      }
    }

    private void ValidateBoolean(LogListener listener, string name)
    {
      if (!listener.Params.ContainsKey(name)) return;

      var value = listener.Params[name];
      if (!string.Equals(value, "true", StringComparison.InvariantCultureIgnoreCase)
          && !string.Equals(value, "false", StringComparison.InvariantCultureIgnoreCase))
      {
        throw new InvalidOperationException(string.Format("{0} parameter must be true of false", name));
      }
    }

  }
}