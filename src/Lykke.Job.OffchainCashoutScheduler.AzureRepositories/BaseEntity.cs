﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Job.OffchainCashoutScheduler.AzureRepositories
{
    public class BaseEntity : TableEntity
    {
        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);

            foreach (var p in GetType().GetProperties().Where(x => x.PropertyType == typeof(decimal) && properties.ContainsKey(x.Name)))
                p.SetValue(this, Convert.ToDecimal(properties[p.Name].StringValue, CultureInfo.InvariantCulture));

            foreach (var p in GetType().GetProperties().Where(x => x.PropertyType.GetTypeInfo().IsEnum && properties.ContainsKey(x.Name)))
                p.SetValue(this, Enum.ToObject(p.PropertyType, properties[p.Name].Int32Value));

        }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var properties = base.WriteEntity(operationContext);

            foreach (var p in GetType().GetProperties().Where(x => x.PropertyType == typeof(decimal)))
                properties.Add(p.Name, new EntityProperty(((decimal)p.GetValue(this)).ToString(CultureInfo.InvariantCulture)));

            foreach (var p in GetType().GetProperties().Where(x => x.PropertyType.GetTypeInfo().IsEnum))
                properties.Add(p.Name, new EntityProperty((int)p.GetValue(this)));

            return properties;
        }
    }
}
