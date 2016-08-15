﻿using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using OneHUDInterface;
using ProjectCars.DataFormat;
using ProjectCars.Readers;
using OneHUDData;

namespace ProjectCars
{
    public class AGProjectCars : GameBase, IGame
    {
        private readonly string _sharedMemoryFileName = "$pcars$";

        private readonly SharedMemoryReader _memoryReader;
        private readonly int _bufferSize = Marshal.SizeOf(typeof(ProjectCars.DataFormat.SharedMemory));
        ProjectCars.DataFormat.SharedMemory _data;

        private readonly CancellationTokenSource _cancel;
        private DateTime _lastTimeStamp;
        private readonly double _pollInterval = 60.0;
        private TelemetryData _telemetryData;
        private TimingData _timingData;
        private bool _connected = false;

        #region Constructor
        public AGProjectCars()
            : base()
        {
            _name = "pCars";
            _displayName = "Project Cars";
            _author = "Alex Greenland";
            _processNames.Add("pCARS");
            _processNames.Add("pCARS64");
            _processNames.Add("pCARS2Gld");
            _memoryReader = new SharedMemoryReader(_sharedMemoryFileName, _bufferSize);
            _cancel = new CancellationTokenSource();
        }
        #endregion

        #region Public Methods
        public override bool Start(TelemetryData telemetryData, TimingData timingData)
        {
            _telemetryData = telemetryData;
            _timingData = timingData;
            ReadData(_cancel.Token);
            return true;
        }

        public override bool Stop()
        {
            _cancel.Cancel();
            return true;
        }
        #endregion

        #region Shared Memory Data Reader
        private async void ReadData(CancellationToken token)
        {
            await Task.Factory.StartNew((Action)(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    if (!_connected)
                    {
                        _connected = _memoryReader.Connect();
                    }

                    DateTime utcNow = DateTime.UtcNow;
                    if ((utcNow - _lastTimeStamp).TotalMilliseconds >= _pollInterval)
                    {
                        _lastTimeStamp = utcNow;
                        byte[] buffer = _memoryReader.Read();

                        GCHandle gCHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                        try
                        {
                            _data = (ProjectCars.DataFormat.SharedMemory)Marshal.PtrToStructure(gCHandle.AddrOfPinnedObject(), typeof(ProjectCars.DataFormat.SharedMemory));
                            ProcessData();
                        }
                        finally
                        {
                            gCHandle.Free();
                        }

                    }
                    Thread.Sleep((int)_pollInterval);
                }
            }));
        }
        #endregion

        #region Process Shared memory data
        private void ProcessData()
        {
            EGameState gameState = (EGameState)_data.MGameState;
            ESessionState sessionState = (ESessionState)_data.MSessionState;

            if (gameState == EGameState.GameIngamePlaying)
            {
                _telemetryData.Car.InCar = true;
            }
            else
            {
                _telemetryData.Car.InCar = false;
            }

            if (_timingData.RaceInfo.TrackLongName == null)
            {
                if (_data.MTrackLocation.Value != "")
                {
                    _timingData.RaceInfo.TrackLongName = _data.MTrackLocation.Value + " " + _data.MTrackVariation.Value;
                    _timingData.RaceInfo.TrackShortName = _data.MTrackLocation.Value;
                    _timingData.RaceInfo.TrackName = _data.MTrackLocation.ToString() + " " + _data.MTrackVariation.Value;
                    _timingData.RaceInfo.TrackVariation = _data.MTrackVariation.Value;
                    _timingData.RaceInfo.TrackLength = (int)_data.MTrackLength;

                    _timingData.RaceInfo.TrackTemperature = _data.MTrackTemperature;
                    _timingData.RaceInfo.AmbientTemperature = _data.MAmbientTemperature;
                }
            }

            if (_telemetryData.Car.InCar)
            {
                _telemetryData.Engine.RPM = _data.MRpm;
                _telemetryData.Car.Speed = ConvertSpeedToMPH(_data.MSpeed);
                _telemetryData.Car.Gear = _data.MGear;
                _telemetryData.Car.FuelRemaining = _data.MFuelLevel * _data.MFuelCapacity;
                _telemetryData.Car.FuelCapacity = _data.MFuelCapacity;

                _telemetryData.Engine.WaterTemp = _data.MWaterTempCelsius;

                _telemetryData.Timing.CurrentLapTime = _data.MCurrentTime;
            }
        }
        #endregion

        #region Helper functions
        private float ConvertSpeedToMPH(float Speed)
        {
            return Speed * (float)2.236936;
        }
        #endregion
    }
}