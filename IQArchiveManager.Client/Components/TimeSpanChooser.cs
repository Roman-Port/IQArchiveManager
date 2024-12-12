using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IQArchiveManager.Client.Components
{
    public partial class TimeSpanChooser : UserControl
    {
        public TimeSpanChooser()
        {
            InitializeComponent();
        }

        public event EventHandler OnValueChanged;
        public TimeSpan MaxValue { get; set; } = TimeSpan.MaxValue;
        public TimeSpan MinValue { get; set; } = TimeSpan.Zero;
        public bool ReadOnly
        {
            get => !valueH.Enabled;
            set
            {
                valueH.Enabled = !value;
                valueM.Enabled = !value;
                valueS.Enabled = !value;
            }
        }

        public TimeSpan Value
        {
            set => ApplyToUi(value);
            get => TimeSpan.FromSeconds((long)valueS.Value + ((long)valueM.Value * 60) + ((long)valueH.Value * 60 * 60));
        }

        private void TimeSpanChooser_Load(object sender, EventArgs e)
        {

        }

        class CustomNumericUpDown : NumericUpDown
        {
            protected override void UpdateEditText()
            {
                Text = Value.ToString().PadLeft(2, '0');
            }
        }

        private void UiValueChanged(object sender, EventArgs e)
        {
            //Check seconds
            while(valueS.Value >= 60)
            {
                valueS.Value -= 60;
                valueM.Value++;
            }
            while (valueS.Value < 0)
            {
                valueS.Value += 60;
                valueM.Value--;
            }

            //Check minutes
            while (valueM.Value >= 60)
            {
                valueM.Value -= 60;
                valueH.Value++;
            }
            while (valueM.Value < 0)
            {
                valueM.Value += 60;
                valueH.Value--;
            }

            //Check constraints
            if (Value > MaxValue)
                Value = MaxValue;
            if (Value < MinValue)
                Value = MinValue;

            //Fire
            OnValueChanged?.Invoke(this, null);
        }

        private void ApplyToUi(TimeSpan value)
        {
            //Temporarily unbind the event (gross)
            valueS.ValueChanged -= UiValueChanged;
            valueM.ValueChanged -= UiValueChanged;
            valueH.ValueChanged -= UiValueChanged;

            //Apply
            long secs = (long)value.TotalSeconds;
            valueS.Value = secs % 60;
            valueM.Value = (secs / 60) % 60;
            valueH.Value = secs / 60 / 60;

            //Rebind the event (gross)
            valueS.ValueChanged += UiValueChanged;
            valueM.ValueChanged += UiValueChanged;
            valueH.ValueChanged += UiValueChanged;
        }

        public void SendUpdateEvent()
        {
            UiValueChanged(this, new EventArgs());
        }
    }
}
