using System;
using System.Data;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using Microsoft.SqlServer.Types;
using System.Data.SqlTypes;
using System.IO;

[Microsoft.SqlServer.Dts.Pipeline.SSISScriptComponentEntryPointAttribute]
public class ScriptMain : UserComponent
{

    MemoryStream ms;
    BinaryWriter bw;

    public override void PreExecute()
    {
        base.PreExecute();

        // Only allocate these once
        ms = new MemoryStream(10000);
        bw = new BinaryWriter(ms);
    }

    public override void Input0_ProcessInputRow(Input0Buffer Row)
    {
        // Create a SqlGeometry object representing the given data
        SqlGeometry g = SqlGeometry.STPointFromText(new SqlChars("POINT (" + Row.Northing + " " + Row.Easting + ")"), 0);

        // Serialize to a memory stream
        ms.SetLength(0);
        g.Write(bw);
        bw.Flush();

        // Copy data from memory stream to output column with DT_IMAGE format
        Row.pt.AddBlobData(ms.GetBuffer(), (int)ms.Length);
    }

}