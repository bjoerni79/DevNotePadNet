using DevNotePad.Shared;
using Generic.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public class RegularExpressionViewModel : MainViewUiViewModel
    {
        public RegularExpressionViewModel()
        {
            Run = new DefaultCommand(OnRun, AreValuesAvailable);
            Close = new DefaultCommand(OnClose);
            Clear = new DefaultCommand(OnClear);
        }

        // Binding Properties

        public string? SearchPattern { get; set; }

        public string? Expression1 { get;set; }

        public string? Expression2 { get;set; }

        public string? Expression3 { get;set; }

        public RegularExpressionResult Result1 { get; set; }

        public RegularExpressionResult Result2 { get; set; }

        public RegularExpressionResult Result3 { get; set; }

        public IRefreshCommand? Run { get; set; }

        public IRefreshCommand? Close { get; set; }

        public IRefreshCommand? Clear { get; set; }

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

            RaisePropertyChange("Expression1");
            RaisePropertyChange("Expression2");
            RaisePropertyChange("Expression3");
            RaisePropertyChange("Result1");
            RaisePropertyChange("Result2");
            RaisePropertyChange("Result3");
            Run.Refresh();
        }

        private void OnRun()
        {
            //TODO
            Result1 = RegularExpressionResult.None;
            Result2 = RegularExpressionResult.Match;
            Result3 = RegularExpressionResult.NoMatch;

            RaisePropertyChange("Result1");
            RaisePropertyChange("Result2");
            RaisePropertyChange("Result3");
        }

        private void OnClose()
        {
            dialog.CloseDialog(true);
        }

    }
}
