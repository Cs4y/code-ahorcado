using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Linq;

class Program
{
    class Jugador
    {
        public string Nombre { get; set; } = string.Empty;
        public int Partidas { get; set; }
        public int Victorias { get; set; }
        public int Derrotas { get; set; }
        public int Puntaje { get; set; }
    }

    class Ranking
    {
        public Jugador[] Jugadores { get; set; } = Array.Empty<Jugador>();
    }

    static string rankingFile = "ranking.json";
    static string bancoFile = "bancodepalabras.txt";
    static Ranking ranking = new Ranking { Jugadores = Array.Empty<Jugador>() };

    static void Main()
    {
        GarantizarBancoDePalabras();
        CargarRanking();
        MostrarMenu();
    }

    static void GarantizarBancoDePalabras()
    {
        if (!File.Exists(bancoFile))
        {
            Console.WriteLine("Optimizando el juego por primera vez (Descargando banco de palabras)...");
            try
            {
                using (var cliente = new System.Net.Http.HttpClient())
                {
                    string url = "https://raw.githubusercontent.com/javierarce/palabras/master/listado-general.txt";
                    string contenido = cliente.GetStringAsync(url).Result;
                    File.WriteAllText(bancoFile, contenido);
                }
            }
            catch (Exception)
            {
                string[] respaldo = { "papa", "mama", "pollo", "casa", "perro", "gato", "mesa", "sopa", "libro", "computadora", "ingeniero", "astronauta", "refrigerador", "bicicleta", "escalera" };
                File.WriteAllLines(bancoFile, respaldo);
            }
        }
    }

    static void CargarRanking()
    {
        if (File.Exists(rankingFile))
        {
            string json = File.ReadAllText(rankingFile);
            if (!string.IsNullOrWhiteSpace(json))
            {
                ranking = JsonSerializer.Deserialize<Ranking>(json) ?? new Ranking { Jugadores = Array.Empty<Jugador>() };
            }
        }

        if (ranking == null || ranking.Jugadores == null)
        {
            ranking = new Ranking();
            ranking.Jugadores = new Jugador[0];
        }
    }

    static void GuardarRanking()
    {
        string json = JsonSerializer.Serialize(ranking, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(rankingFile, json);
    }

    static void MostrarMenu()
    {
        int opcion = 0;

        while (opcion != 3)
        {
            Console.Clear();

            Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                                                                            1║");
            Console.WriteLine("║       █████╗ ██╗  ██╗ ██████╗ ██████╗  ██████╗ █████╗ ██████╗  ██████╗      ║");
            Console.WriteLine("║      ██╔══██╗██║  ██║██╔═══██╗██╔══██╗██╔════╝██╔══██╗██╔══██╗██╔═══██╗      ║");
            Console.WriteLine("║      ███████║███████║██║   ██║██████╔╝██║     ███████║██║  ██║██║   ██║      ║");
            Console.WriteLine("║      ██╔══██║██╔══██║██║   ██║██╔══██╗██║     ██╔══██║██║  ██║██║   ██║      ║");
            Console.WriteLine("║      ██║  ██║██║  ██║╚██████╔╝██║  ██║╚██████╗██║  ██║██████╔╝╚██████╔╝      ║");
            Console.WriteLine("║      ╚═╝  ╚═╝╚═╝  ╚═╝ ╚═════╝ ╚═╝  ╚═╝ ╚═════╝╚═╝  ╚═╝╚═════╝  ╚═════╝       ║");
            Console.WriteLine("║                                                                            ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");

            Console.WriteLine();
            Console.WriteLine("                    ☠ EL JUEGO DEL AHORCADO ☠");
            Console.WriteLine("               ¡Adivina la palabra antes de morir!");
            Console.WriteLine();

            Console.WriteLine("                        _____________");
            Console.WriteLine("                        |           |");
            Console.WriteLine("                        |           O");
            Console.WriteLine("                        |          /|\\");
            Console.WriteLine("                        |          / \\");
            Console.WriteLine("                        |");
            Console.WriteLine("                     ___|________________");
            Console.WriteLine();

            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║              MENÚ PRINCIPAL          ║");
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.WriteLine("║  1 ► Nuevo Juego                     ║");
            Console.WriteLine("║  2 ► Ranking                         ║");
            Console.WriteLine("║  3 ► Salir                           ║");
            Console.WriteLine("╚══════════════════════════════════════╝");

            Console.Write("Seleccione una opción ► ");

            if (!int.TryParse(Console.ReadLine(), out opcion))
                opcion = 0;

            switch (opcion)
            {
                case 1:
                    Console.Clear();
                    NuevoJuego();
                    break;

                case 2:
                    Console.Clear();
                    MostrarRanking();
                    break;

                case 3:
                    Console.Clear();
                    Console.WriteLine();
                    Console.Write("Cerrando juego");
                    for (int i = 0; i < 6; i++)
                    {
                        Thread.Sleep(300);
                        Console.Write(".");
                    }
                    Thread.Sleep(600);
                    break;

                default:
                    Console.Clear();
                    Console.WriteLine("Opción inválida.");
                    Thread.Sleep(1200);
                    break;
            }
        }
    }

    static void NuevoJuego()
    {
        Console.Clear();
        Console.Write("Ingrese el nombre del jugador: ");
        string nombre = Console.ReadLine() ?? "Jugador";
        int dificultad = MenuDificultad();

        if (dificultad == 1) AnimacionFacil();
        else if (dificultad == 2) AnimacionMedia();
        else AnimacionDemonio();

        Console.WriteLine("Iniciando partida...");
        Thread.Sleep(2000);

        string palabraOculta = "";
        string pista = "";

        string[] todasLasPalabras = File.ReadAllLines(bancoFile);
        Random random = new Random();

        if (dificultad == 1)
        {
            var filtradas = todasLasPalabras.Where(p => p.Length >= 4 && p.Length <= 5).ToArray();
            palabraOculta = filtradas[random.Next(filtradas.Length)].ToLower().Trim();
            pista = $"La palabra tiene {palabraOculta.Length} letras.";
        }
        else if (dificultad == 2)
        {
            var filtradas = todasLasPalabras.Where(p => p.Length >= 7 && p.Length <= 10).ToArray();
            palabraOculta = filtradas[random.Next(filtradas.Length)].ToLower().Trim();
            pista = $"La palabra tiene {palabraOculta.Length} letras.";
        }
        else if (dificultad == 3)
        {
            var filtradas = todasLasPalabras.Where(p => p.Length >= 11).ToArray();
            palabraOculta = filtradas[random.Next(filtradas.Length)].ToLower().Trim();
            pista = "Sin pista.";
        }

        char[] palabraActual = new char[palabraOculta.Length];
        for (int i = 0; i < palabraActual.Length; i++)
        {
            palabraActual[i] = '_';
        }

        if (dificultad == 1)
        {
            char primeraLetra = palabraOculta[0];
            for (int i = 0; i < palabraOculta.Length; i++)
            {
                if (palabraOculta[i] == primeraLetra)
                {
                    palabraActual[i] = primeraLetra;
                }
            }
        }

        char[] letrasIngresadas = new char[0];
        int intentos = 4;
        bool ganador = false;

        while (intentos > 0 && !ganador)
        {
            Console.Clear();
            Console.WriteLine("=== JUEGO AHORCADO ===");
            MostrarAhorcado(intentos);
            if (dificultad == 1)
            {
                Console.WriteLine("> Dificultad: Facil");
                Console.WriteLine("> Pista: " + pista);
                Console.WriteLine("> Primera letra: " + palabraOculta[0]);
            }
            else if (dificultad == 2)
            {
                Console.WriteLine("> Dificultad: Media");
                Console.WriteLine("> Pista: " + pista);
            }
            else
            {
                Console.WriteLine("> Dificultad: Dificil");
                Console.WriteLine("> Sin pista.");
            }

            Console.WriteLine("> Palabra: " + new string(palabraActual));
            Console.WriteLine("> Intentos restantes: " + intentos);
            Console.Write("> Letras ingresadas: ");
            for (int i = 0; i < letrasIngresadas.Length; i++)
            {
                Console.Write(letrasIngresadas[i] + " ");
            }
            Console.WriteLine();

            char letra;
            while (true)
            {
                Console.Write("Ingrese una letra: ");
                string ingreso = (Console.ReadLine() ?? "").ToLower();

                if (ingreso.Length == 1 && char.IsLetter(ingreso[0]))
                {
                    letra = ingreso[0];
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("¡Entrada inválida! Solo se permiten letras (A-Z). Intente de nuevo.");
                    Console.ResetColor();
                    Console.WriteLine();
                }
            }

            bool yaIngresada = false;
            for (int i = 0; i < letrasIngresadas.Length; i++)
            {
                if (letrasIngresadas[i] == letra)
                {
                    yaIngresada = true;
                    break;
                }
            }

            if (yaIngresada)
            {
                Console.WriteLine("La letra ya fue ingresada. Presione una tecla para continuar.");
                Console.ReadKey();
                continue;
            }

            char[] nuevoArreglo = new char[letrasIngresadas.Length + 1];
            for (int i = 0; i < letrasIngresadas.Length; i++)
            {
                nuevoArreglo[i] = letrasIngresadas[i];
            }
            nuevoArreglo[nuevoArreglo.Length - 1] = letra;
            letrasIngresadas = nuevoArreglo;

            bool encontrado = false;
            for (int i = 0; i < palabraOculta.Length; i++)
            {
                if (palabraOculta[i] == letra)
                {
                    palabraActual[i] = letra;
                    encontrado = true;
                }
            }

            if (!encontrado)
            {
                intentos--;
            }

            ganador = true;
            for (int i = 0; i < palabraActual.Length; i++)
            {
                if (palabraActual[i] == '_')
                {
                    ganador = false;
                    break;
                }
            }
        }

        bool victoria = false;
        if (intentos > 0)
        {
            victoria = true;
        }

        Console.Clear();
        if (victoria)
        {
           Console.Clear();

Console.ForegroundColor = ConsoleColor.Green;

Console.WriteLine(@"
██╗   ██╗██╗ ██████╗████████╗ ██████╗ ██████╗ ██╗ █████╗
██║   ██║██║██╔════╝╚══██╔══╝██╔═══██╗██╔══██╗██║██╔══██╗
██║   ██║██║██║        ██║   ██║   ██║██████╔╝██║███████║
╚██╗ ██╔╝██║██║        ██║   ██║   ██║██╔══██╗██║██╔══██║
 ╚████╔╝ ██║╚██████╗   ██║   ╚██████╔╝██║  ██║██║██║  ██║
  ╚═══╝  ╚═╝ ╚═════╝   ╚═╝    ╚═════╝ ╚═╝  ╚═╝╚═╝╚═╝  ╚═╝
");

Console.ResetColor();

Console.WriteLine($" ¡Felicidades {nombre}!");
Console.WriteLine($"La palabra era: {palabraOculta}");
        }
        else
        {
   Console.Clear();

Console.ForegroundColor = ConsoleColor.Red;

Console.WriteLine(@"
██████╗ ███████╗██████╗ ██████╗ ██╗███████╗████████╗███████╗
██╔══██╗██╔════╝██╔══██╗██╔══██╗██║██╔════╝╚══██╔══╝██╔════╝
██████╔╝█████╗  ██████╔╝██║  ██║██║███████╗   ██║   █████╗
██╔═══╝ ██╔══╝  ██╔══██╗██║  ██║██║╚════██║   ██║   ██╔══╝
██║     ███████╗██║  ██║██████╔╝██║███████║   ██║   ███████╗
╚═╝     ╚══════╝╚═╝  ╚═╝╚═════╝ ╚═╝╚══════╝   ╚═╝   ╚══════╝
");

Console.ResetColor();

Console.WriteLine($" Has perdido, {nombre}.");
Console.WriteLine($"La palabra era: {palabraOculta}");
        }

        GuardarPartida(nombre, victoria, dificultad);
        Console.WriteLine("Presione una tecla para continuar.");
        Console.ReadKey();
    }

    static int MenuDificultad()
    {
        int opcion = 0;
        while (opcion < 1 || opcion > 3)
        {
            Console.Clear();
            Console.WriteLine("=== SELECCIONE DIFICULTAD ===");
            Console.WriteLine("1. Fácil");
            Console.WriteLine("2. Medio");
            Console.WriteLine("3. Difícil");
            Console.WriteLine("==================================");
            Console.Write("Opción: ");
            string? entrada = Console.ReadLine();
            if (!int.TryParse(entrada ?? "", out opcion))
            {
                opcion = 0;
            }
        }
        return opcion;
    }

    static void GuardarPartida(string nombre, bool victoria, int dificultad)
    {
        Jugador? jugador = null;
        for (int i = 0; i < ranking.Jugadores.Length; i++)
        {
            if (ranking.Jugadores[i].Nombre == nombre)
            {
                jugador = ranking.Jugadores[i];
                break;
            }
        }

        if (jugador == null)
        {
            Jugador[] nuevoArreglo = new Jugador[ranking.Jugadores.Length + 1];
            for (int i = 0; i < ranking.Jugadores.Length; i++)
            {
                nuevoArreglo[i] = ranking.Jugadores[i];
            }
            jugador = new Jugador();
            jugador.Nombre = nombre;
            jugador.Partidas = 0;
            jugador.Victorias = 0;
            jugador.Derrotas = 0;
            jugador.Puntaje = 0;
            nuevoArreglo[nuevoArreglo.Length - 1] = jugador;
            ranking.Jugadores = nuevoArreglo;
        }

        jugador.Partidas++;
        if (victoria)
        {
            jugador.Victorias++;
            if (dificultad == 1) jugador.Puntaje += 10;
            else if (dificultad == 2) jugador.Puntaje += 20;
            else if (dificultad == 3) jugador.Puntaje += 30;
        }
        else
        {
            jugador.Derrotas++;
        }

        GuardarRanking();
    }

    static void MostrarRanking()
    {
        Console.Clear();
        Console.WriteLine("============== RANKING ==============");

        if (ranking.Jugadores.Length == 0)
        {
            Console.WriteLine("Aún no hay jugadores registrados.");
        }
        else
        {
            for (int i = 0; i < ranking.Jugadores.Length; i++)
            {
                Jugador j = ranking.Jugadores[i];
                Console.WriteLine("> Nombre: " + j.Nombre);
                Console.WriteLine("> Partidas: " + j.Partidas);
                Console.WriteLine("> Victorias: " + j.Victorias);
                Console.WriteLine("> Derrotas: " + j.Derrotas);
                Console.WriteLine("> Puntaje: " + j.Puntaje);
                Console.WriteLine("-------------------------------------");
            }
        }

        Console.WriteLine("╔══════════════════════════════════════╗");
        Console.WriteLine("║ 1 ► Volver al menú principal         ║");
        Console.WriteLine("║ 2 ► Borrar todo el historial         ║");
        Console.WriteLine("╚══════════════════════════════════════╝");
        Console.Write("Seleccione una opción ► ");

        string opcion = Console.ReadLine() ?? "";

        if (opcion == "2")
        {
            BorrarRankingManualmente();
        }
    }
    static void BorrarRankingManualmente()
    {
        Console.Clear();
        Console.WriteLine("=== BORRAR HISTORIAL DEL RANKING ===");
        Console.Write("¿Está seguro de que desea borrar todo el historial? (s/n): ");
        string respuesta = (Console.ReadLine() ?? "").ToLower().Trim();

        if (respuesta == "s" || respuesta == "si" || respuesta == "sí")
        {
            ranking.Jugadores = Array.Empty<Jugador>();

            GuardarRanking();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("¡El historial de ranking ha sido borrado con éxito!");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine("Operación cancelada. El ranking no sufrió cambios.");
        }

        Console.WriteLine("Presione una tecla para continuar.");
        Console.ReadKey();
    }

    static void MostrarAhorcado(int intentos)
    {
        Console.WriteLine();
        if (intentos == 4)
        {
            Console.WriteLine("   _______");
            Console.WriteLine("   |     |");
            Console.WriteLine("   |      ");
            Console.WriteLine("   |      ");
            Console.WriteLine("   |      ");
            Console.WriteLine("  _|_");
        }
        if (intentos == 3)
        {
            Console.WriteLine("   _______");
            Console.WriteLine("   |     |");
            Console.WriteLine("   |     O");
            Console.WriteLine("   |      ");
            Console.WriteLine("   |      ");
            Console.WriteLine("  _|_");
        }
        if (intentos == 2)
        {
            Console.WriteLine("   _______");
            Console.WriteLine("   |     |");
            Console.WriteLine("   |     O");
            Console.WriteLine("   |     |\\");
            Console.WriteLine("   |      ");
            Console.WriteLine("  _|_");
        }
        if (intentos == 1)
        {
            Console.WriteLine("   _______");
            Console.WriteLine("   |     |");
            Console.WriteLine("   |     O");
            Console.WriteLine("   |    /|\\");
            Console.WriteLine("   |      ");
            Console.WriteLine("  _|_");
        }
        if (intentos == 0)
        {
            Console.WriteLine("   _______");
            Console.WriteLine("   |     |");
            Console.WriteLine("   |     O");
            Console.WriteLine("   |    /|\\");
            Console.WriteLine("   |    / \\");
            Console.WriteLine("  _|_");
        }
    }

    static void AnimacionDemonio()
    {
        for (int i = 0; i < 3; i++)
        {
            Console.Clear();
            Console.WriteLine(@"
   (◣_◢)
   /|||\
    / \
");
            Thread.Sleep(400);

            Console.Clear();
            Console.WriteLine(@"
   (◣.◢) HA HA HA BUENA SUERTE!
   /|||\
    / \
");
            Thread.Sleep(400);
        }
    }

    static void AnimacionFacil()
    {
        for (int i = 0; i < 3; i++)
        {
            Console.Clear();
            Console.WriteLine(@"
   O      
  /|\
  / \
");
            Thread.Sleep(400);

            Console.Clear();
            Console.WriteLine(@"
  \O/      VAMOSS!!!
   |
  / \
");
            Thread.Sleep(400);
        }
    }

    static void AnimacionMedia()
    {
        for (int i = 0; i < 3; i++)
        {
            Console.Clear();
            Console.WriteLine(@"
   ಠ_ಠ
   /|\
   / \
");
            Thread.Sleep(400);

            Console.Clear();
            Console.WriteLine(@"
  (ಠ‿ಠ) INTENTALO.
   \|/
   / \
");
            Thread.Sleep(400);
        }
    }
}