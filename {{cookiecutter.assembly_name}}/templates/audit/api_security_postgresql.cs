if (auditEvent.Target.Old != null)
    {
        var propValue = property.GetValue( auditEvent.Target.Old );
        if (propValue != null)
        {
            var oldData = Convert.ToBase64String( _dbContext.EncryptData( propValue.ToString() ?? string.Empty ) );
            property.SetValue( auditEvent.Target.Old, oldData );
        }
    }
    var newValue = property.GetValue( auditEvent.Target.New );
    if (newValue != null)
    {
        var newData = Convert.ToBase64String( _dbContext.EncryptData( newValue.ToString() ?? string.Empty ) );
        property.SetValue( auditEvent.Target.New, newData );
    }