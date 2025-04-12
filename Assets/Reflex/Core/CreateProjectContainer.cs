using System.Linq;
using System.Collections.Generic;
using Reflex.Configuration;
using UnityEngine;

namespace Reflex.Core
{
    public static class CreateProjectContainer
    {
        public static Container Create()
        {
            var reflexSettings = Resources.Load<ReflexSettings>(nameof(ReflexSettings));

            var loadAllProjectScopes = reflexSettings != null
                ? reflexSettings.LoadAllProjectScopes
                : true;

            var builder = new ContainerBuilder().SetName("ProjectContainer");
            var projectScopes = loadAllProjectScopes
                ? LoadAllProjectScopes()
                : LoadSingleProjectScope();

            foreach (var projectScope in projectScopes.Where(ps => ps.gameObject.activeSelf))
            {
                projectScope.InstallBindings(builder);
            }

            return builder.Build();
        }

        private static IEnumerable<ProjectScope> LoadAllProjectScopes()
        {
            return Resources.LoadAll<ProjectScope>(string.Empty);
        }

        private static IEnumerable<ProjectScope> LoadSingleProjectScope()
        {
            var projectScope = Resources.Load<ProjectScope>("ProjectScope");
            if (projectScope != null)
            {
                yield return projectScope;
            }
        }
    }
}
