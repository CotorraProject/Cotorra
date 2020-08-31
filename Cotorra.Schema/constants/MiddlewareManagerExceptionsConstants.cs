using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public class MiddlewareManagerExceptionsConstants
    {
        #region Messages
        public const string MSG_ERROR_CREATION = "Ocurrió un error al guardar el registro.";
        public const string MSG_ERROR_UPDATE = "Ocurrió un error al actualizar el registro.";
        public const string MSG_ERROR_DELETE = "Ocurrió un error al eliminar el registro.";
        public const string MSG_ERROR_GET = "Ocurrió un error al obtener los registros.";
        public const string MSG_ERROR_FIND = "Ocurrió un error al buscar los registros.";
        #endregion

        #region Codes
        public const int CODE_ERROR_CREATION = 2001;
        public const int CODE_ERROR_UPDATE = 2002;
        public const int CODE_ERROR_DELETE = 2003;
        public const int CODE_ERROR_GET = 2004;
        public const int CODE_ERROR_FIND = 2005;
        #endregion
    }
}
