# Architecture

    [ USER ]
        |
        v
    [ MIDDLEWARE LAYER ]
    |  RATE LIMITTING  |
    |  AUTHENTICATION  |
        |
        V
    [ CONTROLLERS & CONTRACTS]
        |
        V
    [ SERVICES ]
        |
        V
    [ DBCONTEXT ]
    |      MAPPING     |
    | ENTITY FRAMEWORK |
        |
        V
    [ POSTGRES DB]


