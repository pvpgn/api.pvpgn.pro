using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CharacterEditor;

namespace WebAPI
{
    public static class Helper
    {
        public static string MD5(byte[] bytes)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(bytes);
            return BitConverter.ToString(output).Replace("-", "").ToLower(); // hex string
        }

        public static Item DefaultItemPosition(byte[] bytes)
        {
            var newItem = Item.NewItem(bytes);

            newItem.Id = 0;
            newItem.Location = (uint)Item.ItemLocation.Stored;
            newItem.PositionOnBody = (uint)Item.EquipmentLocation.None;
            newItem.PositionX = 0;
            newItem.PositionY = 0;
            newItem.StorageId = (uint)Item.StorageType.Inventory;

            return newItem;
        }
    }
}