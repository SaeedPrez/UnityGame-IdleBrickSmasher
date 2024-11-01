using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
    [Preserve]
    [ES3PropertiesAttribute("enabled", "color", "range")]
    public class ES3Type_ColorBySpeedModule : ES3Type
    {
        public static ES3Type Instance;

        public ES3Type_ColorBySpeedModule() : base(typeof(ParticleSystem.ColorBySpeedModule))
        {
            Instance = this;
        }

        public override void Write(object obj, ES3Writer writer)
        {
            var instance = (ParticleSystem.ColorBySpeedModule)obj;

            writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
            writer.WriteProperty("color", instance.color, ES3Type_MinMaxGradient.Instance);
            writer.WriteProperty("range", instance.range, ES3Type_Vector2.Instance);
        }

        public override object Read<T>(ES3Reader reader)
        {
            var instance = new ParticleSystem.ColorBySpeedModule();
            ReadInto<T>(reader, instance);
            return instance;
        }

        public override void ReadInto<T>(ES3Reader reader, object obj)
        {
            var instance = (ParticleSystem.ColorBySpeedModule)obj;
            string propertyName;
            while ((propertyName = reader.ReadPropertyName()) != null)
                switch (propertyName)
                {
                    case "enabled":
                        instance.enabled = reader.Read<bool>(ES3Type_bool.Instance);
                        break;
                    case "color":
                        instance.color = reader.Read<ParticleSystem.MinMaxGradient>(ES3Type_MinMaxGradient.Instance);
                        break;
                    case "range":
                        instance.range = reader.Read<Vector2>(ES3Type_Vector2.Instance);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
        }
    }
}