using Microsoft.Xrm.Sdk;
using System;

namespace DataversePlugins.Tools
{
    public class Contexto
    {
        public ITracingService Tracer { get; private set; }
        private readonly IOrganizationServiceFactory Factory;
        private readonly IPluginExecutionContext Context;
        private IOrganizationService ServicioSistema;
        private IOrganizationService ServicioUsuarioEjecutante;
        private IOrganizationService ServicioUsuarioIniciador;
        public IManagedIdentityService ManagedIdentityService;
        private readonly Entity Target;
        /// <returns>
        /// Etapa en la que corre el plugin:
        /// 10: Pre validación,
        /// 20: Pre operación,
        /// 40 o 50: Post operación.
        /// </returns>
        public int Stage { get; private set; }
        /// <returns>
        /// Modo de ejecución del plugin:
        /// true: Sincrónico,
        /// false: Asincrónico.
        /// </returns>
        public bool Sincronico { get; private set; }
        public int Profundidad { get; private set; }

        public Contexto(IServiceProvider serviceProvider)
        {
            Tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            Factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            Context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ManagedIdentityService = (IManagedIdentityService)serviceProvider.GetService(typeof(IManagedIdentityService));
            Target = (Entity)Context.InputParameters["Target"];
            Stage = Context.Stage;
            Sincronico = Context.Mode == 0;
            Profundidad = Context.Depth;
        }


        /// <param name="usuario"></param>
        /// <returns>Obtiene la id del usuario</returns>
        public Guid GetUsuarioId(Usuario usuario)
        {
            switch (usuario)
            {
                case Usuario.Ejecutante: return Context.UserId;
                case Usuario.Iniciador: return Context.InitiatingUserId;
                default: throw new Exception($@"{GetType().Name}: No se permite otro usuario aparte de {Usuario.Ejecutante} o {Usuario.Iniciador}");
            }
        }
        private IOrganizationService GetServicioUsuarioSistema()
        {
            if (ServicioSistema is null)
                ServicioSistema = Factory.CreateOrganizationService(null);
            return ServicioSistema;
        }
        private IOrganizationService GetServicioUsuarioEjecutante()
        {

            if (ServicioUsuarioEjecutante is null)
                ServicioUsuarioEjecutante = Factory.CreateOrganizationService(Context.UserId);
            return ServicioUsuarioEjecutante;
        }
        private IOrganizationService GetServicioUsuarioIniciador()
        {
            if (ServicioUsuarioIniciador is null)
                ServicioUsuarioIniciador = Factory.CreateOrganizationService(Context.InitiatingUserId);
            return ServicioUsuarioIniciador;
        }

        /// <param name="usuario"></param>
        /// <returns>Servicio como sistema, usuario ejecutante o usuario iniciador del plugin</returns>
        public IOrganizationService GetServicioComo(Usuario usuario)
        {
            switch (usuario)
            {
                case Usuario.Sistema:
                    return GetServicioUsuarioSistema();
                case Usuario.Ejecutante:
                    return GetServicioUsuarioEjecutante();
                case Usuario.Iniciador:
                    return GetServicioUsuarioIniciador();
            }
            throw new Exception($"{GetType().Name}: Tipo de usuario no provisto");
        }

        /// <returns>Registro sobre el cual se está corriendo el plugin. Contiene únicamente los campos que hayan cambiado su valor hasta el momento durante la transacción corriente.</returns>
        public Entity GetTarget() => Target;

        /// <returns>Devuelve el tipo de evento que se está ejecutando("Create", "Update", "Delete", etc.)</returns>
        public string GetEvento() => Context.MessageName;

        internal Entity GetPreImagen(string preImageName)
        {
            if (
                Context.PreEntityImages.Contains(preImageName)
                &&
                Context.PreEntityImages[preImageName] is Entity
            )
                return Context.PreEntityImages[preImageName];
            return null;
        }
        internal Entity GetPostImagen(string postImageName)
        {
            if (
                Context.PostEntityImages.Contains(postImageName)
                &&
                Context.PostEntityImages[postImageName] is Entity
            )
                return Context.PostEntityImages[postImageName];
            return null;
        }

        /// <typeparam name="T"></typeparam>
        /// <param name="nombrePreImagen"></param>
        /// <param name="nombreAtributo"></param>
        /// <returns>
        /// Valor del atributo de nombre lógico <i>nombreAtributo</i> del registro sobre el que se está ejecutando el plugin.
        /// Si el evento no es de creación y el atributo no está incluido en el target, lo toma de la preImagen de nombre <i>nombrePreImagen</i>.
        /// </returns>
        public T GetAtributoDeTargetOPreImagen<T>(string nombrePreImagen, string nombreAtributo)
        {
            if (GetEvento() == Evento.Create || Target.Contains(nombreAtributo))
                return Target.GetAttributeValue<T>(nombreAtributo);
            return GetPreImagen(nombrePreImagen).GetAttributeValue<T>(nombreAtributo);
        }

        /// <typeparam name="T"></typeparam>
        /// <param name="nombrePostImagen"></param>
        /// <param name="nombreAtributo"></param>
        /// <returns>
        /// Valor del atributo de nombre lógico <i>nombreAtributo</i> del registro sobre el que se está ejecutando el plugin.
        /// Si el target no lo contiene, lo obtiene de la postImagen con nombre <i>nombrePostImagen</i>.
        /// </returns>
        public T GetAtributoDeTargetOPostImagen<T>(string nombrePostImagen, string nombreAtributo)
        {
            if (Target.Contains(nombreAtributo))
                return Target.GetAttributeValue<T>(nombreAtributo);
            return GetPostImagen(nombrePostImagen).GetAttributeValue<T>(nombreAtributo);
        }

        /// <summary>
        /// Asigna o sobreescribe <i>value</i> en <i>key</i> de SharedVariables del contexto de ejecución.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetVariable(string key, object value)
        {
            Context.SharedVariables[key] = value;
        }

        /// <summary>
        /// Obtiene el valor de <i>key</i> de SharedVariables del contexto o de SharedVariables ParentContext del contexto.  
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetVariable(string key)
        {
            if (Context.SharedVariables.Contains(key))
                return Context.SharedVariables[key];
            if (Context.ParentContext != null && Context.ParentContext.SharedVariables.Contains(key))
                return Context.ParentContext.SharedVariables[key];
            return null;
        }

    }
}
