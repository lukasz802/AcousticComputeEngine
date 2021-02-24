using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using static Compute_Engine.Enums;

namespace Compute_Engine.Elements
{
    [Serializable]
    public abstract class ElementsBase : ICloneable, INotifyPropertyChanged
    {
        protected ElementType _type;
        private int _airflow;
        private string _name;
        private string _comments;
        private bool _include;

        public event PropertyChangedEventHandler PropertyChanged;

        public abstract double[] Attenuation();

        public abstract double[] Noise();

        public object Clone()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, this);
                stream.Position = 0;
                return bf.Deserialize(stream);
            }
        }

        public ElementsBase Parent { get; internal set; }

        public string Comments
        {
            get
            {
                return _comments;
            }
            set
            {
                _comments = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public virtual int AirFlow
        {
            get
            {
                return _airflow;
            }
            set
            {
                if (value < 1)
                {
                    _airflow = 1;
                }
                else
                {
                    _airflow = value;
                }
            }
        }

        public virtual bool IsIncluded
        {
            get
            {
                return _include;
            }
            set
            {
                _include = value;
            }
        }

        public ElementType Type
        {
            get
            {
                return _type;
            }
        }

        public void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
