using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace RDManager
{
    /// <summary>
    /// Copy From: https://social.msdn.microsoft.com/Forums/windowsdesktop/en-US/9095625c-4361-4e0b-bfcf-be15550b60a8/imsrdpclientnonscriptablesendkeys?forum=windowsgeneraldevelopmentissues
    /// </summary>
    internal class MsRdpClientNonScriptableWrapper
    {
        #region Inline Interface Definition
        [InterfaceType(1)]
        [Guid("2F079C4C-87B2-4AFD-97AB-20CDB43038AE")]
        private interface IMsRdpClientNonScriptable_Sendkeys : MSTSCLib.IMsTscNonScriptable
        {
            [DispId(4)]
            string BinaryPassword { get; set; }
            [DispId(5)]
            string BinarySalt { get; set; }
            [DispId(1)]
            string ClearTextPassword { set; }
            [DispId(2)]
            string PortablePassword { get; set; }
            [DispId(3)]
            string PortableSalt { get; set; }

            void NotifyRedirectDeviceChange([ComAliasName("MSTSCLib.UINT_PTR")] uint wParam, [ComAliasName("MSTSCLib.LONG_PTR")] int lParam);
            void ResetPassword();

            unsafe void SendKeys(int numKeys, int* pbArrayKeyUp, int* plKeyData);
        }
        #endregion

        private IMsRdpClientNonScriptable_Sendkeys m_ComInterface;

        public MsRdpClientNonScriptableWrapper(object ocx)
        {
            m_ComInterface = (IMsRdpClientNonScriptable_Sendkeys)ocx;
        }

        // keyScanCodes takes the ScanCodes of the pressed keys (1 byte scancode, offset: 0)
        // (Hello Keyboard Layout Nightmares! Why the ____ does Rdp not send virtual keycodes?)

        // XX...XXSSSSSSSS  *S = 1 Bit of the Scancode
        public void SendKeys(int[] keyScanCodes, bool[] keyReleased)
        {
            if (keyScanCodes.Length != keyReleased.Length) throw new ArgumentException("MsRdpClientNonScriptableWrapper.SendKeys: Arraysize must match");
            // at this point I'd rather stay away from everything that does marshalling or converts bools, so we'll do the
            // conversion this way.

            int[] temp = new int[keyReleased.Length];
            for (int i = 0; i < temp.Length; i++) temp[i] = keyReleased[i] ? 1 : 0;

            unsafe
            {
                fixed (int* pScanCodes = keyScanCodes)
                fixed (int* pKeyReleased = temp)
                {
                    m_ComInterface.SendKeys(keyScanCodes.Length, pKeyReleased, pScanCodes);
                }
            }
        }
    }
}
