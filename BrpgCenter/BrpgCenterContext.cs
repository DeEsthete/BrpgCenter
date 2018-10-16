namespace BrpgCenter
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class BrpgCenterContext : DbContext
    {
        // Контекст настроен для использования строки подключения "BrpgCenterContext" из файла конфигурации  
        // приложения (App.config или Web.config). По умолчанию эта строка подключения указывает на базу данных 
        // "BrpgCenter.BrpgCenterContext" в экземпляре LocalDb. 
        // 
        // Если требуется выбрать другую базу данных или поставщик базы данных, измените строку подключения "BrpgCenterContext" 
        // в файле конфигурации приложения.
        public BrpgCenterContext()
            : base("name=BrpgCenterContext")
        {
        }
        
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Character> Characters { get; set; }
    }
}