using System.Collections.Generic;

namespace TW
{
    public static class DamageMultiplier
    {
        /*
         array de elementos
          -> elemento
            -> elementos fraqueza, porcentagem 
         */

        //Fire,
        //Water,
        //Air,
        //Void,
        //Earth,
        //Plant,
        //Light,
        //Dark

        public static Dictionary<Elements, Dictionary<Elements, float>> table = new Dictionary<Elements, Dictionary<Elements, float>>
        {
            {
                Elements.Fire,
                new Dictionary<Elements, float>
                {
                    {
                        Elements.Fire,
                        1.0f
                    },
                    {
                        Elements.Water,
                        1.0f
                    },
                    {
                        Elements.Air,
                        1.0f
                    },
                    {
                        Elements.Void,
                        1.0f
                    },
                    {
                        Elements.Earth,
                        1.0f
                    },
                    {
                        Elements.Plant,
                        1.0f
                    },
                    {
                        Elements.Light,
                        1.0f
                    },
                    {
                        Elements.Dark,
                        1.0f
                    }
                }
            },
            {
                Elements.Water,
                new Dictionary<Elements, float>
                {
                    {
                        Elements.Fire,
                        1.0f
                    },
                    {
                        Elements.Water,
                        1.0f
                    },
                    {
                        Elements.Air,
                        1.0f
                    },
                    {
                        Elements.Void,
                        1.0f
                    },
                    {
                        Elements.Earth,
                        1.0f
                    },
                    {
                        Elements.Plant,
                        1.0f
                    },
                    {
                        Elements.Light,
                        1.0f
                    },
                    {
                        Elements.Dark,
                        1.0f
                    }
                }
            },
            {
                Elements.Air,
                new Dictionary<Elements, float>
                {
                    {
                        Elements.Fire,
                        1.0f
                    },
                    {
                        Elements.Water,
                        1.0f
                    },
                    {
                        Elements.Air,
                        1.0f
                    },
                    {
                        Elements.Void,
                        1.0f
                    },
                    {
                        Elements.Earth,
                        1.0f
                    },
                    {
                        Elements.Plant,
                        1.0f
                    },
                    {
                        Elements.Light,
                        1.0f
                    },
                    {
                        Elements.Dark,
                        1.0f
                    }
                }
            },
            {
                Elements.Void,
                new Dictionary<Elements, float>
                {
                    {
                        Elements.Fire,
                        1.0f
                    },
                    {
                        Elements.Water,
                        1.0f
                    },
                    {
                        Elements.Air,
                        1.0f
                    },
                    {
                        Elements.Void,
                        1.0f
                    },
                    {
                        Elements.Earth,
                        1.0f
                    },
                    {
                        Elements.Plant,
                        1.0f
                    },
                    {
                        Elements.Light,
                        1.0f
                    },
                    {
                        Elements.Dark,
                        1.0f
                    }
                }
            },
            {
                Elements.Earth,
                new Dictionary<Elements, float>
                {
                    {
                        Elements.Fire,
                        1.0f
                    },
                    {
                        Elements.Water,
                        1.0f
                    },
                    {
                        Elements.Air,
                        1.0f
                    },
                    {
                        Elements.Void,
                        1.0f
                    },
                    {
                        Elements.Earth,
                        1.0f
                    },
                    {
                        Elements.Plant,
                        1.0f
                    },
                    {
                        Elements.Light,
                        1.0f
                    },
                    {
                        Elements.Dark,
                        1.0f
                    }
                }
            },
            {
                Elements.Plant,
                new Dictionary<Elements, float>
                {
                    {
                        Elements.Fire,
                        1.0f
                    },
                    {
                        Elements.Water,
                        1.0f
                    },
                    {
                        Elements.Air,
                        1.0f
                    },
                    {
                        Elements.Void,
                        1.0f
                    },
                    {
                        Elements.Earth,
                        1.0f
                    },
                    {
                        Elements.Plant,
                        1.0f
                    },
                    {
                        Elements.Light,
                        1.0f
                    },
                    {
                        Elements.Dark,
                        1.0f
                    }
                }
            },
            {
                Elements.Light,
                new Dictionary<Elements, float>
                {
                    {
                        Elements.Fire,
                        1.0f
                    },
                    {
                        Elements.Water,
                        1.0f
                    },
                    {
                        Elements.Air,
                        1.0f
                    },
                    {
                        Elements.Void,
                        1.0f
                    },
                    {
                        Elements.Earth,
                        1.0f
                    },
                    {
                        Elements.Plant,
                        1.0f
                    },
                    {
                        Elements.Light,
                        1.0f
                    },
                    {
                        Elements.Dark,
                        1.0f
                    }
                }
            },
            {
                Elements.Dark,
                new Dictionary<Elements, float>
                {
                    {
                        Elements.Fire,
                        1.0f
                    },
                    {
                        Elements.Water,
                        1.0f
                    },
                    {
                        Elements.Air,
                        1.0f
                    },
                    {
                        Elements.Void,
                        1.0f
                    },
                    {
                        Elements.Earth,
                        1.0f
                    },
                    {
                        Elements.Plant,
                        1.0f
                    },
                    {
                        Elements.Light,
                        1.0f
                    },
                    {
                        Elements.Dark,
                        1.0f
                    }
                }
            }
        };
    }
}
