using System;
using System.ComponentModel.DataAnnotations;
using Shop.Core.SharedKernel;

namespace Shop.Core.AppSettings;

public sealed class ConnectionOptions : IAppOptions
{
    static string IAppOptions.ConfigSectionPath => "ConnectionStrings";

    [Required]
    public string EventStoreDbConnection { get; private init; }

}