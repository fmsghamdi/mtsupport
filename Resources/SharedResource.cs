namespace MabatSupportSystem.Resources;

/// <summary>
/// Dummy class for shared resource files.
/// This allows us to use a single set of .resx files for the entire application
/// instead of creating separate resource files for each page.
/// 
/// Usage in Razor Pages:
/// @inject IViewLocalizer Localizer
/// @Localizer["CreateTicket"]
/// </summary>
public class SharedResource
{
    // This class is intentionally empty.
    // It serves as a marker for the localization system.
}
