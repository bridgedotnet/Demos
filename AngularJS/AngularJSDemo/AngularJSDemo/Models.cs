using Bridge;
using System;

namespace PhoneCat
{
    [External]
    public class PhoneModel
    {
        public int Age;
        public string Id;
        public string ImageUrl;
        public string Name;
        public string Snippet;
    }

    [External]
    public class PhoneListScopeModel
    {
        public PhoneModel[] Phones;
        public string OrderProp;
    }

    // We ignore this because it is just a wrapper to the service's 'query'
    // action.
    [External]
    public class PhoneQueryModel
    {
        public PhoneModel[] Query()
        {
            return default(PhoneModel[]);
        }

        public PhoneDetailsModel Get(object phoneId,
            Action<PhoneDetailsModel> phoneTask)
        {
            return default(PhoneDetailsModel);
        }
    }

    [External]
    public class PhoneDetailsScopeModel
    {
        public PhoneDetailsModel Phone;
        public string MainImageUrl;

        public Action<string> SetImage;

        /*
         * One could think on making the SetImage function like this:
         *public void SetImage(string imageUrl)
         *{
         * this.MainImageUrl = imageUrl;
         *}
         * But it will not work with AngularJS: it has to be defined on the
         * scope definition (see above).
         */
    }

    [External]
    public class PhoneDetailsModel
    {
        public string Id;
        public string Name;
        public string AdditionalFeatures;
        public AndroidModel Android;
        public string[] Availability;
        public BatteryInfoModel Battery;
        public CameraInfoModel Camera;
        public ConnectivityInfoModel Connectivity;
        public string Description;
        public DisplayInfoModel Display;
        public HardwareInfoModel Hardware;
        public string[] Images;
        public SizeAndWeightInfoModel SizeAndWeight;
        public StorageInfoModel Storage;
    }

    [External]
    public class AndroidModel
    {
        public string Os;
        public string Ui;
    }

    [External]
    public class BatteryInfoModel
    {
        public string StandbyTime;
        public string TalkTime;
        public string Type;
    }

    [External]
    public class CameraInfoModel
    {
        public string[] Features;
        public string Primart;
    }

    [External]
    public class ConnectivityInfoModel
    {
        public string Bluetooth;
        public string Cell;
        public bool Gps;
        public bool Infrared;
        public string Wifi;
    }

    [External]
    public class DisplayInfoModel
    {
        public string ScreenResolution;
        public string ScreenSize;
        public bool TouchScreen;
    }

    [External]
    public class HardwareInfoModel
    {
        public bool Accelerometer;
        public string AudioJack;
        public string Cpu;
        public bool FmRadio;
        public bool PhysicalKeyboard;
        public string Usb;
    }

    [External]
    public class SizeAndWeightInfoModel
    {
        public string[] Dimensions;
        public string Weight;
    }

    [External]
    public class StorageInfoModel
    {
        public string Flash;
        public string Ram;
    }
}