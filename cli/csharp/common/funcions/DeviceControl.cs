using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace Suprema
{
    public class DeviceControl : FunctionModule
    {
        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get device time", getTime));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set device time", setTime));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Reset device to factory settings", factoryReset));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Reboot device", rebootDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Lock device(disable authentication)", lockDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Unlock device(enable authentication)", unlockDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get WiegandMultiConfig", getWiegandMultiConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set WiegandMultiConfig", setWiegandMultiConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get AllConfig", getAllConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get device capabilities", getDeviceCapabilities));

            return functionList;
        }

        void getTime(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            UInt32 timestamp = 0;
            Console.WriteLine("Trying to get device time");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetDeviceTime(sdkContext, deviceID, out timestamp);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                DateTime currentTime = Util.ConvertFromUnixTimestamp(timestamp);
                Console.WriteLine(">>>> Device time : " + currentTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        void setTime(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Trying to set device time");

            UInt32 timestamp = Convert.ToUInt32(Util.ConvertToUnixTimestamp(DateTime.Now));
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetDeviceTime(sdkContext, deviceID, timestamp);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        void factoryReset(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Trying to reset device to factory settings.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_FactoryReset(sdkContext, deviceID);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        void rebootDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Trying to reboot device.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_RebootDevice(sdkContext, deviceID);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        void lockDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Trying to lock device.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_LockDevice(sdkContext, deviceID);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        void unlockDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Trying to unlock device.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_UnlockDevice(sdkContext, deviceID);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        void getWiegandMultiConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2WiegandMultiConfig config;
            Console.WriteLine("Trying to get wiegandMultiConfig");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetWiegandMultiConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(sdkContext, config);                
            }
        }

        void setWiegandMultiConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2WiegandMultiConfig wiegandMultiConfig = Util.AllocateStructure<BS2WiegandMultiConfig>();
            for(int i = 0; i < 15; ++i)
            {
                wiegandMultiConfig.formats[i].formatID = 0;
                wiegandMultiConfig.formats[i].format.length = 0;
                Array.Clear(wiegandMultiConfig.formats[i].format.idFields, 0, 128);
                Array.Clear(wiegandMultiConfig.formats[i].format.parityFields, 0, 128);
                Array.Clear(wiegandMultiConfig.formats[i].format.parityType, 0, 4);                                                                                                                                                   
                Array.Clear(wiegandMultiConfig.formats[i].format.parityPos, 0, 4);
            }

            wiegandMultiConfig.formats[0].formatID = 1;                                    
            wiegandMultiConfig.formats[0].format.length = 26;
            wiegandMultiConfig.formats[0].format.idFields[28] = 0x01;
            wiegandMultiConfig.formats[0].format.idFields[29] = 0xFE;

            wiegandMultiConfig.formats[0].format.idFields[31 + 29] = 0x01;
            wiegandMultiConfig.formats[0].format.idFields[31 + 30] = 0xFF;
            wiegandMultiConfig.formats[0].format.idFields[31 + 31] = 0xFE;

            wiegandMultiConfig.formats[0].format.parityType[0] = 2;
            wiegandMultiConfig.formats[0].format.parityPos[0] = 0;

            wiegandMultiConfig.formats[0].format.parityType[1] = 1;
            wiegandMultiConfig.formats[0].format.parityPos[1] = 25;

            wiegandMultiConfig.formats[0].format.parityType[2] = 0;
            wiegandMultiConfig.formats[0].format.parityPos[2] = 0;

            wiegandMultiConfig.formats[0].format.parityType[3] = 0;
            wiegandMultiConfig.formats[0].format.parityPos[3] = 0;

            wiegandMultiConfig.formats[0].format.parityFields[28] = 0x01;
            wiegandMultiConfig.formats[0].format.parityFields[29] = 0xFE;
            wiegandMultiConfig.formats[0].format.parityFields[30] = 0xE0;

            wiegandMultiConfig.formats[0].format.parityFields[31 + 30] = 0x1F;
            wiegandMultiConfig.formats[0].format.parityFields[31 + 31] = 0xFE;

            Console.WriteLine("Trying to set wiegandMultiConfig");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetWiegandMultiConfig(sdkContext, deviceID, ref wiegandMultiConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                getWiegandMultiConfig(sdkContext, deviceID, true);
            }
        }
        

        void print(IntPtr sdkContext, BS2WiegandMultiConfig wiegandMultiConfig)
        {
            for (int idx = 0; idx < 15; ++idx)
            {
                Console.WriteLine(">>>> WiegandMulti configuration formatID[{0}] = {1}", idx, wiegandMultiConfig.formats[idx].formatID);
                Console.WriteLine("     |--length : {0}", wiegandMultiConfig.formats[idx].format.length);
                for (int j = 0; j < 4; ++j)
                {
                    for(int k = 27; k < 32; ++k)
                    {
                        Console.WriteLine("     |--idFields[{0},{1}] : {2}", j, k, wiegandMultiConfig.formats[idx].format.idFields[(j*31)+k]);
                    }
                }
            }
        }

        void getAllConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2Configs configs = Util.AllocateStructure<BS2Configs>();
            configs.configMask = (uint)BS2ConfigMaskEnum.ALL;
            Console.WriteLine("Trying to get AllConfig");

            Type structureType = typeof(BS2Configs);
            int structSize = Marshal.SizeOf(structureType);

            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetConfig(sdkContext, deviceID, ref configs);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("BS2_GetConfig failed. Error : {0}", result);
            }
            else
            {
                Console.WriteLine("BS2_GetConfig Success  : {0}", configs.factoryConfig.deviceID);
            }
        }

        void getDeviceCapabilities(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2DeviceCapabilities capa = Util.AllocateStructure<BS2DeviceCapabilities>();

            Console.WriteLine("Trying to get device capabilities");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetDeviceCapabilities(sdkContext, deviceID, out capa);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(capa);
            }
        }

        void print(BS2DeviceCapabilities info)
        {
            Console.WriteLine(">>>> Device capabilities");
	        Console.WriteLine("     |--maxUsers : {0}", info.maxUsers);
	        Console.WriteLine("     |--maxEventLogs : {0}", info.maxEventLogs);
	        Console.WriteLine("     |--maxImageLogs : {0}", info.maxImageLogs);
	        Console.WriteLine("     |--maxBlacklists : {0}", info.maxBlacklists);
	        Console.WriteLine("     |--maxOperators : {0}", info.maxOperators);
	        Console.WriteLine("     |--maxCards : {0}", info.maxCards);
	        Console.WriteLine("     |--maxFaces : {0}", info.maxFaces);
	        Console.WriteLine("     |--maxFingerprints : {0}", info.maxFingerprints);
	        Console.WriteLine("     |--maxUserNames : {0}", info.maxUserNames);
	        Console.WriteLine("     |--maxUserImages : {0}", info.maxUserImages);
	        Console.WriteLine("     |--maxUserJobs : {0}", info.maxUserJobs);
	        Console.WriteLine("     |--maxUserPhrases : {0}", info.maxUserPhrases);
	        Console.WriteLine("     |--maxCardsPerUser : {0}", info.maxCardsPerUser);
	        Console.WriteLine("     |--maxFacesPerUser : {0}", info.maxFacesPerUser);
	        Console.WriteLine("     |--maxFingerprintsPerUser : {0}", info.maxFingerprintsPerUser);
	        Console.WriteLine("     |--maxInputPorts : {0}", info.maxInputPorts);
	        Console.WriteLine("     |--maxOutputPorts : {0}", info.maxOutputPorts);
	        Console.WriteLine("     |--maxRelays : {0}", info.maxRelays);
	        Console.WriteLine("     |--maxRS485Channels : {0}", info.maxRS485Channels);

            Console.WriteLine("     |--cameraSupported : {0}", Convert.ToBoolean(info.systemSupported & (byte)BS2CapabilitySystemSupport.SYSTEM_SUPPORT_CAMERA));
            Console.WriteLine("     |--tamperSupported : {0}", Convert.ToBoolean(info.systemSupported & (byte)BS2CapabilitySystemSupport.SYSTEM_SUPPORT_TAMPER));
            Console.WriteLine("     |--wlanSupported : {0}", Convert.ToBoolean(info.systemSupported & (byte)BS2CapabilitySystemSupport.SYSTEM_SUPPORT_WLAN));
            Console.WriteLine("     |--displaySupported : {0}", Convert.ToBoolean(info.systemSupported & (byte)BS2CapabilitySystemSupport.SYSTEM_SUPPORT_DISPLAY));
            Console.WriteLine("     |--thermalSupported : {0}", Convert.ToBoolean(info.systemSupported & (byte)BS2CapabilitySystemSupport.SYSTEM_SUPPORT_THERMAL));
            Console.WriteLine("     |--maskSupported : {0}", Convert.ToBoolean(info.systemSupported & (byte)BS2CapabilitySystemSupport.SYSTEM_SUPPORT_MASK));
            Console.WriteLine("     |--faceExSupported : {0}", Convert.ToBoolean(info.systemSupported & (byte)BS2CapabilitySystemSupport.SYSTEM_SUPPORT_FACEEX));

	        Console.WriteLine("     |--[Card Supported]");
	        Console.WriteLine("     |--mask : 0x{0}", Convert.ToString(info.cardSupportedMask, 16));
            Console.WriteLine("     |--BLE : {0}", Convert.ToBoolean(info.cardSupportedMask & (UInt32)BS2CapabilityCardSupport.CARD_SUPPORT_BLE));

            bool extMode = Convert.ToBoolean(info.authSupported.extendedMode);
	        Console.WriteLine("     |--[ExtendedMode] : {0}", extMode);
	        Console.WriteLine("     |--(Credentials)");
	        Console.WriteLine("     |--mask : 0x{0}", Convert.ToString(info.authSupported.credentialsMask, 16));

	        if (!extMode)
	        {
                BS2LagacyAuth auth = Util.BytesToStruct<BS2LagacyAuth>(ref info.authSupported.auth);

                Console.WriteLine("     |--(Legacy)");
                Console.WriteLine("     |--biometricAuthMask : 0x{0}", Convert.ToString(auth.biometricAuthMask, 16));
                Console.WriteLine("     |--cardAuthMask : 0x{0}", Convert.ToString(auth.cardAuthMask, 16));
                Console.WriteLine("     |--idAuthMask : 0x{0}", Convert.ToString(auth.idAuthMask, 16));
	        }
	        else
	        {
                BS2ExtendedAuth auth = Util.BytesToStruct<BS2ExtendedAuth>(ref info.authSupported.auth);
 
                Console.WriteLine("     |--(Extended)");
		        Console.WriteLine("     |--faceAuthMask : 0x{0}", Convert.ToString(auth.faceAuthMask, 16));
		        Console.WriteLine("     |--fingerprintAuthMask : 0x{0}", Convert.ToString(auth.fingerprintAuthMask, 16));
		        Console.WriteLine("     |--cardAuthMask : 0x{0}", Convert.ToString(auth.cardAuthMask, 16));
		        Console.WriteLine("     |--idAuthMask : 0x{0}", Convert.ToString(auth.idAuthMask, 16));
                Console.WriteLine("     |--faceAuth : 0x{0}", Convert.ToString(auth.faceAuthMask, 16));
	        }

            Console.WriteLine("     |--intelligentPDSupported : {0}", Convert.ToBoolean(info.functionSupported & (byte)BS2CapabilityFunctionSupport.FUNCTION_SUPPORT_INTELLIGENTPD));
            Console.WriteLine("     |--smartCardByteOrderSupported : {0}", Convert.ToBoolean(info.functionSupported & (byte)BS2CapabilityFunctionSupport.FUNCTION_SUPPORT_SMARTCARDBYTEORDER));
            Console.WriteLine("     |--treatAsCSNSupported : {0}", Convert.ToBoolean(info.functionSupported & (byte)BS2CapabilityFunctionSupport.FUNCTION_SUPPORT_TREATASCSN));
        }
    }
}
