using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace K3yManager.Works
{
    class Elfworks
    {
        enum ident_4{ELFCLASSNONE ,ELFCLASS32 , ELFCLASS64 } ; 
        enum ident_5 {ELFDATANONE , ELFDATA2LSB , ELFDATA2MSB }; 
        public struct Elf_header
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst =16)]
            public byte[] e_ident; 
            ushort e_type;
            ushort e_machine;
            uint e_version;
            uint e_entry;
            uint e_phoff;
            uint e_shoff;
            uint e_flags;
            ushort e_ehsize;
            ushort e_phentsize;
            ushort e_phnum;
            ushort e_shentsize;
            ushort e_shnum;
            ushort e_shstrndx;
        }
       public  struct Elf32_Shdr{
        uint sh_name;
        uint sh_type;
        uint sh_flags;
        uint sh_addr;
        uint sh_offset;
        uint sh_size;
        uint sh_link;
        uint sh_info;
        uint sh_addralign;
        uint sh_entsize;
    }



    }
}
