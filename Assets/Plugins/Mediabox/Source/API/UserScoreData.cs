using System;

namespace Mediabox.API {
    /// <summary>
    /// This contains the format of
    /// e.g. <see cref="Mediabox.GameKit.GameManager.GameManagerBase"/>
    /// </summary>
    [Serializable]
    public struct UserScoreData
    {
        public float value;

        public UserScoreData(float value)
        {
            this.value = value;
        }
    }
}