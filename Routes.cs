using Orchard.Mvc.Routes;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Moov2.Orchard.FindReplace {
    public class Routes : IRouteProvider {
        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (RouteDescriptor routeDescriptor in GetRoutes()) {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "Admin/FindReplace",
                        new RouteValueDictionary {
                            {"area", "Moov2.Orchard.FindReplace"},
                            {"controller", "Admin"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Moov2.Orchard.FindReplace"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "Admin/FindReplace/Preview",
                        new RouteValueDictionary {
                            {"area", "Moov2.Orchard.FindReplace"},
                            {"controller", "Admin"},
                            {"action", "Preview"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Moov2.Orchard.FindReplace"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "Admin/FindReplace/Replace",
                        new RouteValueDictionary {
                            {"area", "Moov2.Orchard.FindReplace"},
                            {"controller", "Admin"},
                            {"action", "Replace"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Moov2.Orchard.FindReplace"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
    }
}
