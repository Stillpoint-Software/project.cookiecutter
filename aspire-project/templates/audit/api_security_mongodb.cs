if (auditEvent.Target.Old != null)
        {
            var propValue = property.GetValue( auditEvent.Target.Old );
            if (propValue != null)
            {
                property.SetValue( auditEvent.Target.Old, SecurityHelper.EncryptValue( propValue.ToString() ?? string.Empty ) );
            }
        }
        var newValue = property.GetValue( auditEvent.Target.New );
        if (newValue != null)
        {
            property.SetValue( auditEvent.Target.New, SecurityHelper.DecryptValue( newValue.ToString() ?? string.Empty ) );
        }