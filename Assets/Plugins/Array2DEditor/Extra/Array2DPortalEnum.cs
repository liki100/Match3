using UnityEngine;

namespace Array2DEditor
{
    [System.Serializable]
    public class Array2DPortalEnum : Array2D<PortalType>
    {
        [SerializeField]
        CellRowPortalEnum[] cells = new CellRowPortalEnum[Consts.defaultGridSize];

        protected override CellRow<PortalType> GetCellRow(int idx)
        {
            return cells[idx];
        }
    }

    [System.Serializable]
    public class CellRowPortalEnum : CellRow<PortalType> { }


}