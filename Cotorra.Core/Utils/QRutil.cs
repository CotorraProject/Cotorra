using QRCoder;
using System;
using System.Collections.Generic;
 
using System.DrawingCore;
using System.Text;

namespace Cotorra.Core.Utils
{
    public class QRutil
    {
        public byte[] GetQr(string data, int pixelsPerModule)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(pixelsPerModule);
            return ImageToByte(qrCodeImage);
        }

        public string GetQRBase64(string data, int pixelsPerModule)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            return qrCode.GetGraphic(pixelsPerModule);
        }

        private static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
    }
}
