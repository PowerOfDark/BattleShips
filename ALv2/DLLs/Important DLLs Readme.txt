Folders in this directory contain the NetworkComms.Net DLLs for supported platforms.

For some platforms we have included merged DLLs which can be used in isolation,
i.e. all dependencies are included, significantly reducing the number of assembly
references required in your own projects when using NetworkComms.Net

DLLs that are found in the folders named 'Individual' are the equivalent unmerged
DLLs and may have further dependencies which have not been included to reduce the
total bundle size. If you choose to use these unmerged DLLs these dependencies can
be easily downloaded from www.NuGet.org and may include the following:
  \ 32Feet.NET - InTheHand.Net.Personal.dll
  \ Json.NET - Newtonsoft.Json.dll
  \ protobuf-net - protobuf-net.dll
