using FHIR_Concept_Map_Builder.Forms;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace FHIR_Concept_Map_Builder;

public static class ConceptMapBuilder
{
    public static string ToJson(NHSNLinkEncounterFormViewModel viewModel)
    {
        var map = new ConceptMap();
        map.Id = $"{viewModel.SiteId}-encounter-type";
        map.Name = $"{viewModel.SiteName} Encounter.Type ConceptMap";
        map.Url = $"https://nhsnlink.org/fhir/ConceptMap/{map.Id}";
        map.Version = DateTime.Now.ToString("yyyyMMdd");
        map.Title = $"{viewModel.SiteName} Encounter.Type ConceptMap";
        map.Status = PublicationStatus.Draft;
        map.Experimental = true;
        map.Date = DateTime.Now.ToString("yyyy-MM-dd");
        map.Description = $"A mapping between the {viewModel.SiteName} encounter type codes and SNOMED CT codes";
        map.Purpose = $"To help implementers map encounters from {viewModel.SiteName} to SNOMED CT";
        map.Group = new List<ConceptMap.GroupComponent>();
        ConceptMap.GroupComponent group = new ConceptMap.GroupComponent();
        if (viewModel.Vendor == EHRVendor.Epic)
        {
            group.Source = $"urn:oid:1.2.840.114350.1.13.{viewModel.EpicSiteIdentifier}.2.7.10.698084.10110";
        }
        else if (viewModel.Vendor == EHRVendor.Cerner)
        {
            group.Source = $"https://fhir.cerner.com/{viewModel.CernerSiteIdentifier}/codeSet/71";
        }
        else
        {
            group.Source = viewModel.CodeMap.SourceCode;
        }
        group.Target = "http://snomed.info/sct";
        group.Element = new List<ConceptMap.SourceElementComponent>();
        splitMapping(viewModel.CodeMap, group);
        fillEncounterDisplay(group);
        map.Group.Add(group);
        if (viewModel.Vendor == EHRVendor.Epic)
        {
            ConceptMap.GroupComponent groupSandbox = new ConceptMap.GroupComponent();
            groupSandbox.Source = $"urn:oid:1.2.840.114350.1.13.{viewModel.EpicSiteIdentifier}.3.7.10.698084.10110";
            groupSandbox.Target = "http://snomed.info/sct";
            groupSandbox.Element = new List<ConceptMap.SourceElementComponent>();
            splitMapping(viewModel.CodeMap, groupSandbox);
            fillEncounterDisplay(groupSandbox);
            map.Group.Add(groupSandbox);
        }

        return map.ToJson(true);
    }

    public static string ToJson(NHSNLinkLocationFormViewModel viewModel)
    {
        var map = new ConceptMap();
        map.Id = $"{viewModel.SiteId}-location-type";
        map.Name = $"{viewModel.SiteName} Location.Type ConceptMap";
        map.Url = $"https://nhsnlink.org/fhir/ConceptMap/{map.Id}";
        map.Version = DateTime.Now.ToString("yyyyMMdd");
        map.Title = $"{viewModel.SiteName} Location.Type ConceptMap";
        map.Status = PublicationStatus.Draft;
        map.Experimental = true;
        map.Date = DateTime.Now.ToString("yyyy-MM-dd");
        map.Description = $"A mapping between the {viewModel.SiteName} location codes and CDC HSLoc codes";
        map.Purpose = $"To help implementers map locations from {viewModel.SiteName} to CDC HSLoc";
        map.Group = new List<ConceptMap.GroupComponent>();
        ConceptMap.GroupComponent group = new ConceptMap.GroupComponent();
        if (viewModel.Vendor == EHRVendor.Epic)
        {
            group.Source = $"urn:oid:1.2.840.114350.1.13.{viewModel.EpicSiteIdentifier}.2.7.2.686980";
        }
        else if (viewModel.Vendor == EHRVendor.Cerner)
        {
            group.Source = "https://nhsnlink.org/location-alias";
        }
        else
        {
            group.Source = viewModel.CodeMap.SourceCode;
        }
        group.Target = "https://www.cdc.gov/nhsn/cdaportal/terminology/codesystem/hsloc.html";
        group.Element = new List<ConceptMap.SourceElementComponent>();
        splitMapping(viewModel.CodeMap, group);
        fillLocationDisplay(group);
        map.Group.Add(group);
        if (viewModel.Vendor == EHRVendor.Epic)
        {
            ConceptMap.GroupComponent groupSandbox = new ConceptMap.GroupComponent();
            groupSandbox.Source = $"urn:oid:1.2.840.114350.1.13.{viewModel.EpicSiteIdentifier}.3.7.2.686980";
            groupSandbox.Target = "https://www.cdc.gov/nhsn/cdaportal/terminology/codesystem/hsloc.html";
            groupSandbox.Element = new List<ConceptMap.SourceElementComponent>();
            splitMapping(viewModel.CodeMap, groupSandbox);
            fillLocationDisplay(groupSandbox);
            map.Group.Add(groupSandbox);
        }

        return map.ToJson(true);
    }

    private static void fillLocationDisplay(ConceptMap.GroupComponent group)
    {
        foreach (var element in group.Element)
        {
            var target = element.Target.FirstOrDefault();
            if (target == null) continue;
            if (string.IsNullOrEmpty(target.Display) && CodeDictionaries.HsLocCodes.ContainsKey(target.Code))
            {
                target.Display = CodeDictionaries.HsLocCodes[target.Code];
            }
        }
    }

    private static void fillEncounterDisplay(ConceptMap.GroupComponent group)
    {
        foreach (var element in group.Element)
        {
            var target = element.Target.FirstOrDefault();
            if (target == null) continue;
            if (string.IsNullOrEmpty(target.Display) && CodeDictionaries.SnomedCodes.ContainsKey(target.Code))
            {
                target.Display = CodeDictionaries.SnomedCodes[target.Code];
            }
        }
    }

    public static string ToJson(ConceptMapViewModel viewModel)
    {
        var map = new ConceptMap();
        map.Id = viewModel.ID;
        map.Name = viewModel.Name;
        map.Url = viewModel.URL;
        map.Version = viewModel.Version;
        map.Title = viewModel.Title;
        map.Status = viewModel.Status;
        map.Experimental = viewModel.Experimental;
        map.Date = viewModel.Date;
        map.Description = viewModel.Description;
        map.Purpose = viewModel.Purpose;
        map.Group = new List<ConceptMap.GroupComponent>();
        foreach (var codeMap in viewModel.CodeMaps)
        {
            ConceptMap.GroupComponent group = new ConceptMap.GroupComponent();
            group.Source = codeMap.SourceCode;
            group.Target = codeMap.TargetCode;
            group.Element = new List<ConceptMap.SourceElementComponent>();
            if (string.IsNullOrEmpty(codeMap.Mapping)) continue;
            splitMapping(codeMap, group);
            map.Group.Add(group);
        }
        return map.ToJson(true);
    }

    private static void splitMapping(CodeMapViewModel codeMap, ConceptMap.GroupComponent group)
    {
        var maps = codeMap.Mapping.Split(Environment.NewLine);
        foreach (var mapping in maps)
        {
            var parts = mapping.Split("\t");
            if (parts.Length < 2) continue;
            var elementComponent = new ConceptMap.SourceElementComponent();
            elementComponent.Code = parts[0];
            elementComponent.Target = new List<ConceptMap.TargetElementComponent>();
            var target = new ConceptMap.TargetElementComponent();
            target.Code = parts[1];
            if (parts.Length > 2) {
                target.Display = parts[2];
            }
            elementComponent.Target.Add(target);
            group.Element.Add(elementComponent);
        };
    }
}

public class ConceptMapViewModel
{
    public string? ID { get; set; }
    public string? Name { get; set; }
    public string? URL { get; set; }
    public string? Version { get; set; }
    public string? Title { get; set; }
    public PublicationStatus Status { get; set; }
    public bool Experimental { get; set; }
    public string? Date { get; set; }
    public string? Description { get; set; }
    public string? Purpose { get; set; }

    public List<CodeMapViewModel> CodeMaps { get; set; } = new List<CodeMapViewModel>() { new CodeMapViewModel() };
}

public class CodeMapViewModel
{
    public bool Expanded { get; set; } = true;
    public string? SourceCode { get; set; }
    public string? TargetCode { get; set; }
    public string? Mapping { get; set; }
}

public class NHSNLinkLocationFormViewModel
{
    public string? SiteName { get; set; }
    public string? SiteId { get; set; }
    public EHRVendor? Vendor { get; set; }
    public string? EpicSiteIdentifier { get; set; }

    public CodeMapViewModel CodeMap { get; set; } = new CodeMapViewModel();
}

public class NHSNLinkEncounterFormViewModel
{
    public string? SiteName { get; set; }
    public string? SiteId { get; set; }
    public EHRVendor? Vendor { get; set; }
    public string? EpicSiteIdentifier { get; set; }
    public string? CernerSiteIdentifier { get; set; }

    public CodeMapViewModel CodeMap { get; set; } = new CodeMapViewModel();
}

public enum EHRVendor
{
    Epic,
    Cerner,
    Other
}
