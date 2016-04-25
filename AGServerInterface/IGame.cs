﻿using System;
using System.Collections.Generic;
using OneHUDData;

namespace OneHUDInterface
{
    public interface IGame
    {

        string Name { get; }

        string DisplayName { get; }

        List<string> ProcessNames { get; }

        string Version { get; }

        bool Start(TelemetryData telemetryData);

        bool Stop();
    }
}