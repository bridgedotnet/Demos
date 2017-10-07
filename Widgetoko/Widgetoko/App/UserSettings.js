Bridge.assembly("Widgetoko", function ($asm, globals) {
    "use strict";

    Bridge.define("Widgetoko.MainProcess.UserSettings", {
        $literal: true,
        statics: {
            methods: {
                Deserialize: function (data) {
                    var deobfuscatedDataArr = System.Convert.fromBase64String(data);
                    var serializedData = System.Text.Encoding.Default.GetString(deobfuscatedDataArr);
                    var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(serializedData, System.Object);

                    return obj;
                }
            }
        },
        methods: {
            Serialize: function () {
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(this);
                var dataArr = System.Text.Encoding.Default.GetBytes$2(data);
                var obfuscatedData = System.Convert.toBase64String(dataArr, null, null, null);

                return obfuscatedData;
            }
        }
    });
});
