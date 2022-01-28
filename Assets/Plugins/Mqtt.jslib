var MqttWebService = {
    $mqttState: {
        /* Event listeners */
        client: null,
        onConnected: null,
        onDisconnected: null,
        onConnectingFailed: null,
        onMsgPublishReceived: null,
        currentTopic:null,
        currentMsg:null,
        /* Debug mode */
        debug: false
    },

    /**
     * Set onConnected callback
     *
     * @param callback Reference to C# static function
     */
    MqttSetConnected: function (callback) {

        mqttState.onConnected = callback;
    },

    /**
     * Set onDisconnected callback
     *
     * @param callback Reference to C# static function
     */
    MqttSetDisconnected: function (callback) {

        mqttState.onDisconnected = callback;
    },

    /**
     * Set onConnectingFailed callback
     *
     * @param callback Reference to C# static function
     */
    MqttSetConnectFailed: function (callback) {

        mqttState.onConnectingFailed = callback;

    },

    /**
     * Set onClose callback
     *
     * @param callback Reference to C# static function
     */
    MqttSetMsgPublishReceived: function (callback) {

        mqttState.onMsgPublishReceived = callback;

    },

    MqttConnect: function (_clentId, _host, _userName, _password) {
        if(mqttState.client != null){
            mqttState.client.end(true);
        }
        var clientId = Pointer_stringify(_clentId);
        var host = Pointer_stringify(_host);
        var userName = Pointer_stringify(_userName);
        var passWord = Pointer_stringify(_password);
        const options = {
            keepalive: 10,
            clientId: clientId,
            protocolId: 'MQTT',
            protocolVersion: 4,
            clean: true,
            reconnectPeriod: 1000,
            connectTimeout: 30 * 1000,
            username: userName,
            password: passWord,
            will: {
                topic: 'WillMsg',
                payload: 'Connection Closed abnormally..!',
                qos: 0,
                retain: false
            },
        }
        mqttState.client = mqtt.connect(host, options);
        mqttState.client.on('connect', function () {
            if (mqttState.onConnected)
                Runtime.dynCall('v', mqttState.onConnected);
        });

        mqttState.client.on('disconnect', function (packet) {
            if (mqttState.onDisconnected)
                Runtime.dynCall('v', mqttState.onDisconnected);
        });

        mqttState.client.on('message', function (topic, message, packet) {
            if (mqttState.onMsgPublishReceived) {
                var bufferTopicSize = lengthBytesUTF8(topic) + 1;
                mqttState.currentTopic = _malloc(bufferTopicSize);
                stringToUTF8(topic, mqttState.currentTopic, bufferTopicSize);

                const msg = message.join(',');
                var bufferMsgSize = lengthBytesUTF8(msg) + 1;
                mqttState.currentMsg = _malloc(bufferMsgSize);
                stringToUTF8(msg, mqttState.currentMsg, bufferMsgSize);
                try{
                    Runtime.dynCall('vii', mqttState.onMsgPublishReceived, [mqttState.currentTopic, mqttState.currentMsg]);
                }finally {
                    _free(mqttState.currentTopic);
                    _free(mqttState.currentMsg);
                }
            }
        });
    },
    
    Subscribe: function (_topic, qos) {
        var topic = Pointer_stringify(_topic);
        var opt = {
            qos:parseInt(qos)
        };
        mqttState.client.subscribe(topic, opt);
    },

    UnSubscribe: function (topics) {
        var topicArrs = Pointer_stringify(topics).split("|");
        mqttState.client.unsubscribe(topicArrs);
    },

    CloseClient:function (){
        mqttState.client.end()
    },

    Publish:function (_topic, _message, _qos){
        var topic = Pointer_stringify(_topic);
        var message = Pointer_stringify(_message).split("|");
        var buffers = new Uint8Array(message.length);
        for (var i = 0; i < buffers.length; i++) {
            buffers[i] = parseInt(message[i]);
        }
        var opt = {qos:_qos};
        mqttState.client.publish(topic, buffers, opt);
    }

};

autoAddDeps(MqttWebService, '$mqttState');
mergeInto(LibraryManager.library, MqttWebService);