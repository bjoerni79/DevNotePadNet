using CommunityToolkit.Mvvm.Input;
using DevNotePad.Shared;
using Generic.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public class RegularExpressionViewModel : MainViewUiViewModel
    {
        private string? expression1;
        private string? expression2;
        private string? expression3;

        public RegularExpressionViewModel()
        {
            Run = new RelayCommand(OnRun, AreValuesAvailable);
            Close = new RelayCommand(OnClose);
            Clear = new RelayCommand(OnClear);
        }

        // Binding Properties

        public string? SearchPattern { get; set; }

        public string? Expression1
        {
            get { return expression1; }
            set { expression1 = value; Run.NotifyCanExecuteChanged(); }
        }

        public string? Expression2
        {
            get { return expression2; }
            set { expression2 = value; Run.NotifyCanExecuteChanged(); }
        }

        public string? Expression3
        {
            get { return expression3; }
            set { expression3 = value; Run.NotifyCanExecuteChanged(); }
        }

        public RegularExpressionResult Result1 { get; set; }

        public RegularExpressionResult Result2 { get; set; }

        public RegularExpressionResult Result3 { get; set; }

        public RelayCommand Run { get; set; }

        public RelayCommand Close { get; set; }

        public RelayCommand Clear { get; set; }

        public bool IgnoreCaseOption { get; set; }

        public bool MultiLineOption { get; set; }

        public bool SingleLineOption { get; set; }

        public bool IgnorePatternWhitespaces { get; set; }

        private bool AreValuesAvailable()
        {
            var isSearchPatternAvailable = !String.IsNullOrEmpty(SearchPattern);
            var isExpression1Available = !String.IsNullOrEmpty(Expression1);
            var isExpression2Available = !String.IsNullOrEmpty(Expression2);
            var isExpression3Available = !String.IsNullOrEmpty(Expression3);


            return isSearchPatternAvailable && (isExpression1Available || isExpression2Available || isExpression3Available);
        }

        private void OnClear()
        {
            Expression1 = null;
            Expression2 = null;
            Expression3 = null;
            Result1 = RegularExpressionResult.None;
            Result2 = RegularExpressionResult.None;
            Result3 = RegularExpressionResult.None;

            OnPropertyChanged("Expression1");
            OnPropertyChanged("Expression2");
            OnPropertyChanged("Expression3");
            OnPropertyChanged("Result1");
            OnPropertyChanged("Result2");
            OnPropertyChanged("Result3");
            Run.NotifyCanExecuteChanged();
        }

        private void OnRun()
        {
            Result1 = Perform(Expression1, SearchPattern);
            Result2 = Perform(Expression2, SearchPattern);
            Result3 = Perform(Expression3, SearchPattern);

            OnPropertyChanged("Result1");
            OnPropertyChanged("Result2");
            OnPropertyChanged("Result3");
        }

        private RegexOptions BuildOptions()
        {
            RegexOptions options = new RegexOptions();
            if (IgnoreCaseOption)
            {
                options |= RegexOptions.IgnoreCase;
            }

            if (MultiLineOption)
            {
                options |= RegexOptions.Multiline;
            }

            if (SingleLineOption)
            {
                options |= RegexOptions.Singleline;
            }

            if (IgnorePatternWhitespaces)
            {
                options |= RegexOptions.IgnorePatternWhitespace;
            }

            return options;
        }

        private RegularExpressionResult Perform(string expression, string pattern)
        {
            var options = BuildOptions();
            var returnValue = RegularExpressionResult.None;

            try
            {
                var match = Regex.Match(expression, pattern, options);
                if (match.Success)
                {
                    if (match.Value.Length > 0)
                    {
                        returnValue = RegularExpressionResult.Match;
                    }
                    else
                    {
                        returnValue = RegularExpressionResult.NoMatch;
                    }
                    
                }
                else
                {
                    returnValue = RegularExpressionResult.NoMatch;
                }
            }
            catch (ArgumentException argumentEx)
            {
                //TODO
            }

            return returnValue;
        }

        private void OnClose()
        {
            dialog.CloseDialog(true);
        }

    }
}
