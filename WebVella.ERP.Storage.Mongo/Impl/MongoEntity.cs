﻿using System;
using System.Collections.Generic;

namespace WebVella.ERP.Storage.Mongo
{
    internal class MongoEntity : MongoDocumentBase, IStorageEntity
    {
        public string Name { get; set; }

        public string Label { get; set; }

        public string LabelPlural { get; set; }

        public bool System { get; set; }

        public string IconName { get; set; }

        public decimal Weight { get; set; }

        public IStorageRecordPermissions RecordPermissions { get; set; }

        public List<IStorageField> Fields { get; set; }

        public List<IStorageRecordsList> RecordsLists { get; set; }

        public List<IStorageRecordView> RecordViewList { get; set; }

        public MongoEntity()
        {
            Fields = new List<IStorageField>();
            RecordsLists = new List<IStorageRecordsList>();
            RecordViewList = new List<IStorageRecordView>();
            RecordPermissions = new MongoRecordPermissions();
        }
    }

    public class MongoRecordPermissions : IStorageRecordPermissions
    {
        public List<Guid> CanRead { get; set; }

        public List<Guid> CanCreate { get; set; }

        public List<Guid> CanUpdate { get; set; }

        public List<Guid> CanDelete { get; set; }

        public MongoRecordPermissions()
        {
            CanRead = new List<Guid>();
            CanCreate = new List<Guid>();
            CanUpdate = new List<Guid>();
            CanDelete = new List<Guid>();
        }
    }
}