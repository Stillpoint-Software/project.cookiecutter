using Audit.Core;
using System.Collections;

namespace {{cookiecutter.assembly_name}}.Api.Infrastructure;
public class ListAuditEvent : AuditEvent
{
    public ListAuditEvent( IList list ) => List = list;

    public IList List { get; set; }

}