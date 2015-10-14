using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using u4 = System.UInt32;
using u2 = System.UInt16;
using u1 = System.Char;
using System.IO;
using System.Runtime.InteropServices;

namespace K3yManager.Works
{
    class DexWorks
    {
        private Eenclass enc = new Eenclass();
        // ?no need 
        private string[] header_real = new string[23]; 

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct header_item
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] magic  ;
            public uint checksum;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] siganature;
            public uint file_size;
            public uint header_size;
            public uint endian_tag;
            public uint link_size;
            public uint link_off;
            public uint map_off;
            public uint string_ids_size;
            public uint string_ids_off;
            public uint type_ids_size;
            public uint type_ids_off;
            public uint proto_ids_size;
            public uint proto_ids_off;
            public uint field_ids_size;
            public uint field_ids_off; 
            public uint method_ids_size;
            public uint method_ids_off;
            public uint class_defs_size;
            public uint class_defs_off;
            public uint data_size;
            public uint data_off;
        }

        public struct maplist
        {
           public uint size;
           public map_item[] list; 
        }

        public struct map_item
        {
          public  ushort type;
          public  ushort unuse;
          public  uint size;
          public  uint offset; 
        }


        public struct string_ids_item
        {
            public uint size;
            public strcontent[] strlist;  
        } 

        public struct strcontent
        {
            public uint len;
            public string content; 
        }

        
        private byte[] src  ; 
        private string path ;
        object structType = null;
        private header_item head;
        public maplist Maplist; 

        public DexWorks(string path)
        {
            if(path ==null)
            {
                return; 
            }
            this.path = path;
            FileStream fs = new FileStream(path, FileMode.Open);
            src= new byte[fs.Length];
            fs.Read(src, 0, src.Length);
            fs.Close(); 
        }

        public void AnaMapoff()
        {
            int size = src[head.map_off]; 
           
            Maplist = new maplist();
            Maplist.size = (uint)size; 
            Maplist.list = new map_item[size];
            for (int i = 0; i < size; ++i)
            {
                structType = Maplist.list[i]; 
                ByteArrayToStructure(src, ref structType, (int)head.map_off + 4+i*12);
                Maplist.list[i] = (map_item)structType;
            }

         
        }

        private void findoffandsize(ref uint offset , ref uint size,uint type)
        {
            for (int i = 0; i < Maplist.size; i++)
            {
                if (Maplist.list[i].type == type)
                {
                    offset = Maplist.list[i].offset;
                    size = Maplist.list[i].size;
                }
            }
        }
        public void AnaStringoff()
        {
            uint offset = 0;
            uint items = 0;
            findoffandsize(ref offset, ref items, 0x0001); 
            for (int i = 0; i <items; i++)
            {
                uint string_off = (uint)src[offset + i * 4];
                 
            }

        }


        public string[] AnaHeader()
        {
            
            string[] result = new string[23];
            head = new header_item();
            structType = head; 
            ByteArrayToStructure(src, ref structType, 0);
            head = (header_item)structType;
            if(head.magic[0]!=0x64)
            {
                return null; 
            }
            result[0] = header_real[0] = Encoding.Default.GetString(head.magic);
            result[1] = header_real[1] = Convert.ToString(head.checksum,16);
            result[2] = header_real[2] = enc.hex2str(head.siganature); 
            result[3] = header_real[3] = Convert.ToString(head.file_size,16);
            result[4] = header_real[4] = Convert.ToString(head.header_size,16);
            result[5] = header_real[5] = Convert.ToString(head.endian_tag,16);
            result[6] = header_real[6] = Convert.ToString(head.link_size,16);
            result[7] = header_real[7] = Convert.ToString(head.link_off,16);
            result[8] = header_real[8] = Convert.ToString(head.map_off,16);
            result[9] = header_real[9] = Convert.ToString(head.string_ids_size,16);
            result[10] = header_real[10] = Convert.ToString(head.string_ids_off,16);
            result[11] = header_real[11] = Convert.ToString(head.type_ids_size,16);
            result[12] = header_real[12] = Convert.ToString(head.type_ids_off,16);
            result[13] = header_real[13] = Convert.ToString(head.proto_ids_size,16);
            result[14] = header_real[14] = Convert.ToString(head.proto_ids_off,16);
            result[15] = header_real[15] = Convert.ToString(head.field_ids_size,16);
            result[16] = header_real[16] = Convert.ToString(head.field_ids_off,16);
            result[17] = header_real[17] = Convert.ToString(head.method_ids_size,16);
            result[18] = header_real[18] = Convert.ToString(head.method_ids_off,16);
            result[19] = header_real[19] = Convert.ToString(head.class_defs_size,16);
            result[20] = header_real[20] = Convert.ToString(head.class_defs_off,16);
            result[21] = header_real[21] = Convert.ToString(head.data_size,16);
            result[22] = header_real[22] = Convert.ToString(head.data_off,16);
            return result; 
        }


        byte[] StructureToByteArray(object obj)
        {
            int len = Marshal.SizeOf(obj);
            byte[] arr = new byte[len];
            IntPtr ptr = Marshal.AllocHGlobal(len);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);
            Marshal.FreeHGlobal(ptr);
            return arr; 

        }

         public static void ByteArrayToStructure(byte[] bytearray, ref object obj, int startoffset)
        {
            int len = Marshal.SizeOf(obj);
            IntPtr i = Marshal.AllocHGlobal(len);
          //  obj = Marshal.PtrToStructure(i, obj.GetType());
            try
            {
                Marshal.Copy(bytearray, startoffset, i, len);
            }
            catch(Exception)
            {  }

            obj = Marshal.PtrToStructure(i, obj.GetType());
            Marshal.FreeHGlobal(i); 

        }

    }
}
