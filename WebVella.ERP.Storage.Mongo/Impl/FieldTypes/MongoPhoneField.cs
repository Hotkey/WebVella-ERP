﻿using WebVella.ERP.Storage;

namespace WebVella.ERP.Storage.Mongo
{
    public class MongoPhoneField : MongoBaseField, IStoragePhoneField
    {
        public string DefaultValue { get; set; }

        public string Format { get; set; }

        public int? MaxLength { get; set; }
    }
}