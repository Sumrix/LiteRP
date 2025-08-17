using System.Diagnostics.CodeAnalysis;
using LiteRP.WebApp.Components.Icons;

namespace LiteRP.WebApp.Helpers;

public class IconTypes
{
    public static readonly Type DotsVertical = typeof(DotsVerticalIcon);
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(UploadSolidIcon))]
    public static readonly Type UploadSolid = typeof(UploadSolidIcon);
    public static readonly Type Blocks = typeof(BlocksIcon);
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(TrashBinIcon))]
    public static readonly Type TrashBin = typeof(TrashBinIcon);
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PaperPlaneIcon))]
    public static readonly Type PaperPlane = typeof(PaperPlaneIcon);
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(QuestionCircleSolidIcon))]
    public static readonly Type QuestionCircleSolid = typeof(QuestionCircleSolidIcon);
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SidenavIcon))]
    public static readonly Type Sidenav = typeof(SidenavIcon);
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(CloseIcon))]
    public static readonly Type Close = typeof(CloseIcon);
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(EyeSolidIcon))]
    public static readonly Type EyeSolid = typeof(EyeSolidIcon);
    public static readonly Type InfoCircleSolid = typeof(InfoCircleSolidIcon);
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(StopSolidIcon))]
    public static readonly Type StopSolid = typeof(StopSolidIcon);
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(MessageDotsSolidIcon))]
    public static readonly Type MessageDotsSolid = typeof(MessageDotsSolidIcon);
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ProfileCardSolidIcon))]
    public static readonly Type ProfileCardSolid = typeof(ProfileCardSolidIcon);
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(InfoCircleIcon))]
    public static readonly Type InfoCircle = typeof(InfoCircleIcon);
}