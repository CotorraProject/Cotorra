using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    public enum EmployeeIdentityRegistrationStatus
    {
        /// <summary>
        /// 1. Inicial, se crea el registro del empleado  y se le genera un código de activación
        /// </summary>
        Initial = 1,

        /// <summary>
        /// Se le manda el correo el correo al empleado invitandolo a usar la app móvil
        /// </summary>
        MailSent = 2,

        /// <summary>
        /// 3. El usuario crea su identidad
        /// </summary>
        IdentityCreated = 3,

        /// <summary>
        /// 4. Activa su cuenta de identidades con su id de empleado
        /// </summary>
        Completed = 4        
    }
}
