using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Helpers
{
   public interface IEmailHelper
    {
        void SendUserCreatedEmail(string AppUser, string password, string correoDestino);

        void SendVerificationEmail(string correoDestino, string tokenVerificacion);
        void SendVerificationEmail(string correoDestino, string tokenVerificacion, string verificarBaseUrl);
        void SendVerificationEmail(string correoDestino, string tokenVerificacion, string verificarBaseUrl, int? tenantId);
        string RenderVerificationPreview(int? tenantId);
        void SendEmailUpdateNotification(string correoDestino, string tokenVerificacion,DateTime fechaActulizacion, string correoNuevo);
        void SendAccessDeniedEmail(string correoDestino, string motivo);
        void SendExternalUserAccessEmail(ExternalUser importador);
        void SendRoleCreatedEmail(string AppUser, string NombreRol);
        void SendRoleEditedEmail(string AppUser, string NombreRol);
        void SendRequestUpdateEmail(List<string> correoDestino, string motivo, string TemporaryCode);
    }
}
