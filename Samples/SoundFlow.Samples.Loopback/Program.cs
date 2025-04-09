using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Providers;
using SoundFlow.Structs;

namespace SoundFlow.Samples.Loopback;

public static class LoopbackExample
{
    public static void Main(string[] args)
    {
        // 1. Initialize Audio Engine (MiniAudio Backend)
        using var audioEngine = new MiniAudioEngine(44100, Capability.Mixed, SampleFormat.F32, 1);

        // 2. List Available Devices
        Console.WriteLine("Available Playback Devices:");
        DeviceInfo[] playbackDevices = audioEngine.PlaybackDevices;
        for (int i = 0; i < playbackDevices.Length; i++)
        {
            Console.WriteLine($"[{i}] {playbackDevices[i].Name} (Default: {playbackDevices[i].IsDefault})");
        }

        Console.WriteLine("\nAvailable Capture Devices:");
        DeviceInfo[] captureDevices = audioEngine.CaptureDevices;
        for (int i = 0; i < captureDevices.Length; i++)
        {
            Console.WriteLine($"[{i}] {captureDevices[i].Name} (Default: {captureDevices[i].IsDefault})");
        }

        // 3. User Device Selection
        Console.Write("\nEnter the number for your Microphone (Capture) Device: ");
        if (!int.TryParse(Console.ReadLine(), out var micDeviceIndex) || micDeviceIndex < 0 || micDeviceIndex >= captureDevices.Length)
        {
            Console.WriteLine("Invalid microphone device index.");
            return;
        }
        DeviceInfo selectedMicDevice = captureDevices[micDeviceIndex];

        Console.Write("Enter the number for your VB-Cable (Playback) Device (or similar virtual audio cable): ");
        DeviceInfo selectedVbCableDevice;
        while (true) // Loop to ensure valid VB-Cable selection
        {
            if (!int.TryParse(Console.ReadLine(), out var vbcableDeviceIndex) || vbcableDeviceIndex < 0 || vbcableDeviceIndex >= playbackDevices.Length)
            {
                Console.WriteLine("Invalid VB-Cable device index. Please enter a valid number:");
            }
            else
            {
                selectedVbCableDevice = playbackDevices[vbcableDeviceIndex];
                break; // Exit loop if valid index
            }
        }


        // 4. Switch to Selected Devices
        try
        {
            audioEngine.SwitchDevices(selectedVbCableDevice, selectedMicDevice);
            Console.WriteLine($"\nUsing Microphone: {selectedMicDevice.Name}");
            Console.WriteLine($"Outputting to VB-Cable: {selectedVbCableDevice.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error switching devices: {ex.Message}");
            return;
        }

        // 5. Create Microphone Data Provider
        using var microphoneDataProvider = new MicrophoneDataProvider();
        microphoneDataProvider.StartCapture();

        // 6. Create Sound Player for Output to VB-Cable
        var soundPlayer = new SoundPlayer(microphoneDataProvider);
        Mixer.Master.AddComponent(soundPlayer);

        // 7. Start Playback (Routing)
        soundPlayer.Play();
        Console.WriteLine("\nMicrophone routing started. Audio from your selected microphone is now being routed to VB-Cable.");
        Console.WriteLine("Press Enter to stop routing.");
        Console.ReadLine();

        // 8. Stop and Dispose
        microphoneDataProvider.StopCapture();
        soundPlayer.Stop();
        Mixer.Master.RemoveComponent(soundPlayer);

        Console.WriteLine("Microphone routing stopped.");
    }
}