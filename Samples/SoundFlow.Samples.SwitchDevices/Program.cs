using SoundFlow.Abstracts;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Providers;

namespace SoundFlow.Samples.SwitchDevices;

internal static class Program
{
    private static readonly AudioEngine Engine = new MiniAudioEngine(44100, Capability.Playback);
    
    private static void Main()
    {
        EnumerateDevices();
        Console.WriteLine("\n\nEnter audio file path:");
        
        var path = Console.ReadLine()?.Replace("\"", "");
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            Console.WriteLine("File not found");
            Console.ReadLine();
            return;
        }
        
        var dataProvider = new StreamDataProvider(File.OpenRead(path));
        var player = new SoundPlayer(dataProvider);
        Mixer.Master.AddComponent(player);
        player.Play();

        Console.WriteLine("Press any key to exit or press 'g' to change device or press 'r' to update devices list.");
        while (true)
        {
            var key = Console.ReadKey().KeyChar;
            if (key == 'g')
            {
                Console.WriteLine("\nSelect device:");
                for (var i = 0; i < Engine.PlaybackDeviceCount; i++)
                {
                    Console.WriteLine($"{i}: {Engine.PlaybackDevices[i].Name}");
                }

                Console.WriteLine("Press any key to exit.");
                var choice = Console.ReadKey().KeyChar;
                if (int.TryParse(choice.ToString(), out var index) && index >= 0 && index < Engine.PlaybackDeviceCount)
                    Engine.SwitchDevice(Engine.PlaybackDevices[index]);
                Console.WriteLine($"\nCurrent device: {Engine.PlaybackDevices[index].Name}");
                Console.WriteLine("Press any key to exit or press 'g' to change device or press 'r' to update devices list.");
            }
            else if (key == 'r')
            {
                Engine.UpdateDevicesInfo();
                EnumerateDevices();
            }
            else
                break;
        }

        Mixer.Master.RemoveComponent(player);
    }

    private static void EnumerateDevices()
    {
        // Enumerate devices
        Console.WriteLine($"\nAvailable playback devices: {Engine!.PlaybackDeviceCount}");
        foreach (var device in Engine.PlaybackDevices)
        {
            Console.WriteLine($"Device: {device.Name} - Is Default ({device.IsDefault})");
        }

        Console.WriteLine($"\nAvailable capture devices: {Engine.CaptureDeviceCount}");
        foreach (var device in Engine.CaptureDevices)
        {
            Console.WriteLine($"Device: {device.Name} - Is Default ({device.IsDefault})");
        }
    }
}

