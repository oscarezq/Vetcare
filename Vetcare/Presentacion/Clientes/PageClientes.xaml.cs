using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Clientes
{
    /// <summary>
    /// Página encargada de mostrar, filtrar, ordenar y gestionar los clientes registrados en el sistema.
    /// </summary>
    public partial class PageClientes : Page
    {
        // Lista que almacena todos los clientes de la tabla clientes de la base de datos vetcare
        private List<Cliente> listaCompleta = new List<Cliente>();

        // Servicio de la capa de negocio para la gestión de operaciones de clientes.
        ClienteService cs = new ClienteService();

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="PageClientes"/>.
        /// </summary>
        public PageClientes()
        {
            InitializeComponent();

            // Mostrar los datos en la tabla
            CargarDatos();
        }

        /// <summary>
        /// Recupera la información de los clientes desde la Capa de Negocio.
        /// </summary>
        private void CargarDatos()
        {
            try
            {
                // Invocación al servicio para obtener la colección completa
                listaCompleta = cs.ObtenerTodos();

                // Refresca la UI con los datos obtenidos
                ActualizarTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar clientes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Método que procesa la lista de clientes aplicando los filtros de texto y los criterios de ordenación.
        /// </summary>
        private void ActualizarTabla()
        {
            try
            {
                // Verificamos que la lista original no sea nula
                if (listaCompleta == null)
                    return;

                // Verificamos que los controles de la interfaz existan para evitar errores al iniciar
                if (rbAsc == null || cbOrdenarPor == null)
                    return;

                // FILTRADO
                // Lista con los clientes que cumplen los filtros
                List<Cliente> listaFiltrada = new List<Cliente>();

                // Recorremos todos los clientes de la lista completa
                foreach (Cliente c in listaCompleta)
                {
                    // Convertimos a minúsculas los campos de texto
                    string dniBusca = txtBuscaNumDocumento.Text.ToLower();
                    string clienteBusca = txtBuscaCliente.Text.ToLower();
                    string telBusca = txtBuscaTelefono.Text.ToLower();
                    string emailBusca = txtBuscaEmail.Text.ToLower();
                    string dirBusca = txtBuscaDireccion.Text.ToLower();

                    // Comprobamos cada filtro. Si no cumple alguno, pasamos al siguiente cliente
                    if (!string.IsNullOrEmpty(dniBusca) && !c.NumDocumento.ToLower().Contains(dniBusca)) continue;
                    if (!string.IsNullOrEmpty(clienteBusca) && !c.NombreCompleto.ToLower().Contains(clienteBusca)) continue;
                    if (!string.IsNullOrEmpty(telBusca) && !c.Telefono.ToLower().Contains(telBusca)) continue;
                    if (!string.IsNullOrEmpty(emailBusca) && !c.Email.ToLower().Contains(emailBusca)) continue;
                    if (!string.IsNullOrEmpty(dirBusca) && !c.Direccion.ToLower().Contains(dirBusca)) continue;

                    // Filtro especial para la fecha (Desde - Hasta)
                    // Si existe fecha "Desde"
                    if (dtpBuscaFechaDesde.SelectedDate.HasValue)
                    {
                        // Si la fecha del cliente es anterior a la fecha desde, no cumple el filtro
                        if (c.FechaAlta.Date < dtpBuscaFechaDesde.SelectedDate.Value.Date)
                            continue;
                    }

                    // Si existe fecha "Hasta"
                    if (dtpBuscaFechaHasta.SelectedDate.HasValue)
                    {
                        // Si la fecha del cliente es posterior a la fecha hasta, no cumple el filtro
                        if (c.FechaAlta.Date > dtpBuscaFechaHasta.SelectedDate.Value.Date)
                            continue;
                    }

                    // Si ha llegado hasta aquí, es que cumple todos los filtros. Por lo tanto, se añade a la lista
                    listaFiltrada.Add(c);
                }

                //ORDENACIÓN
                // Obtenemos el campo de ordenación seleccionado
                ComboBoxItem itemSeleccionado = (ComboBoxItem)cbOrdenarPor.SelectedItem;

                // Comprobamos que el campo seleccionado no es nulo
                if (itemSeleccionado != null)
                {
                    // Comprobar que ya existe la tabla en memoria
                    if (dgClientes == null)
                        return;

                    // Obtenemos la cadena con el campo a filtrar
                    string criterio = itemSeleccionado.Content.ToString();

                    // Orden de ordenación:
                    // Si el checkbox rbAsc está seleccionado, el orden ascendente
                    // Si no está seleccionado, el orden es descendente
                    bool esAscendente = (bool)rbAsc.IsChecked;

                    // Evaluamos el criterio de ordenación seleccionado en el ComboBox
                    switch (criterio)
                    {
                        // Si el usuario eligió ordenar por el documento de identidad
                        case "DNI":
                            if (esAscendente) // Si la dirección es de menor a mayor
                                              // Comparamos el DNI de x con el de y
                                listaFiltrada.Sort((x, y) => x.NumDocumento.CompareTo(y.NumDocumento));
                            else // Si la dirección es de mayor a menor
                                 // Comparamos el DNI de y con el de x para invertir el orden
                                listaFiltrada.Sort((x, y) => y.NumDocumento.CompareTo(x.NumDocumento));

                            break;

                        // Si el usuario eligió ordenar por el nombre de pila
                        case "Nombre":
                            if (esAscendente) // De la A a la Z
                                              // Comparamos los nombres alfabéticamente (x primero)
                                listaFiltrada.Sort((x, y) => x.Nombre.CompareTo(y.Nombre));
                            else // De la Z a la A
                                 // Comparamos los nombres alfabéticamente (y primero)
                                listaFiltrada.Sort((x, y) => y.Nombre.CompareTo(x.Nombre));

                            break;

                        // Si el usuario eligió ordenar por los apellidos
                        case "Apellidos":
                            if (esAscendente) // Orden alfabético normal
                                              // Ejecuta la comparación basada en la cadena de apellidos de x e y
                                listaFiltrada.Sort((x, y) => x.Apellidos.CompareTo(y.Apellidos));
                            else // Orden alfabético inverso
                                 // Ejecuta la comparación basada en la cadena de apellidos de y e x
                                listaFiltrada.Sort((x, y) => y.Apellidos.CompareTo(x.Apellidos));

                            break;

                        // Si el usuario eligió ordenar por el número de contacto
                        case "Teléfono":
                            if (esAscendente) // De menor a mayor valor numérico/alfabético
                                              // Realiza la comparativa entre los números de teléfono
                                listaFiltrada.Sort((x, y) => x.Telefono.CompareTo(y.Telefono));
                            else // De mayor a menor valor
                                 // Realiza la comparativa inversa entre los números de teléfono
                                listaFiltrada.Sort((x, y) => y.Telefono.CompareTo(x.Telefono));

                            break;

                        // Si el usuario eligió ordenar por la dirección de correo
                        case "Email":
                            if (esAscendente) // Ordenar correos de la A a la Z
                                              // Compara las cadenas de texto de los correos electrónicos
                                listaFiltrada.Sort((x, y) => x.Email.CompareTo(y.Email));
                            else // Ordenar correos de la Z a la A
                                 // Compara las cadenas de texto de los correos electrónicos de forma inversa
                                listaFiltrada.Sort((x, y) => y.Email.CompareTo(x.Email));

                            break;

                        // Si el usuario eligió ordenar por el domicilio registrado
                        case "Dirección":
                            if (esAscendente) // Orden alfabético de calles/ciudades
                                              // Compara las direcciones como cadenas de caracteres
                                listaFiltrada.Sort((x, y) => x.Direccion.CompareTo(y.Direccion));
                            else // Orden alfabético inverso de calles/ciudades
                                 // Compara las direcciones de forma inversa para invertir el listado
                                listaFiltrada.Sort((x, y) => y.Direccion.CompareTo(x.Direccion));

                            break;

                        // Si el usuario eligió ordenar por el momento de registro en el sistema
                        case "Fecha de Alta":
                            if (esAscendente) // De la fecha más antigua a la más reciente
                                              // Compara los objetos DateTime de fecha_alta de x e y
                                listaFiltrada.Sort((x, y) => x.FechaAlta.CompareTo(y.FechaAlta));
                            else // De la fecha más reciente a la más antigua
                                 // Compara los objetos DateTime de fecha_alta de y e x
                                listaFiltrada.Sort((x, y) => y.FechaAlta.CompareTo(x.FechaAlta));

                            break;
                    }
                }

                // Enviamos la lista resultante a la tabla
                dgClientes.ItemsSource = listaFiltrada;
            } 
            catch (Exception ex) 
            {
                // Capturamos cualquier error inesperado
                MessageBox.Show($"Error al editar cliente: {ex.Message}", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Método que se ejecuta cada vez que el usuario escribe o borra texto en cualquiera de los cuadros de búsqueda.
        /// Llama al método que actualiza la tabla con los filtros.
        /// </summary>
        private void FiltroAvanzado_Changed(object sender, TextChangedEventArgs e) => ActualizarTabla();

        /// <summary>
        /// Método que se ejecuta cuando el usuario selecciona una fecha en el calendario.
        /// Llama al método que actualiza la tabla con los filtros.
        /// </summary>
        private void dtpBuscaFecha_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => ActualizarTabla();

        /// <summary>
        /// Método que se ejecuta al cambiar la opción del desplegable de campos para ordenar.
        /// Llama al método que actualiza la tabla con los filtros.
        /// </summary>
        private void cbOrdenarPor_SelectionChanged(object sender, SelectionChangedEventArgs e) => ActualizarTabla();

        /// <summary>
        /// Método que se ejecuta al marcar los RadioButtons de Ascendente o Descendente.
        /// Llama al método que actualiza la tabla con los filtros.
        /// </summary>
        private void OrdenDirection_Checked(object sender, RoutedEventArgs e) => ActualizarTabla();

        /// <summary>
        /// Método que se ejecuta al pulsar el botón de "Limpiar Filtros" para resetear todo el panel de búsqueda y volver al estado inicial.
        /// </summary>
        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaCliente.Clear();
            txtBuscaTelefono.Clear();
            txtBuscaEmail.Clear();
            txtBuscaDireccion.Clear();
            dtpBuscaFechaDesde.SelectedDate = null;
            dtpBuscaFechaHasta.SelectedDate = null;
            cbOrdenarPor.SelectedIndex = 0;
            rbAsc.IsChecked = true;
            ActualizarTabla();
        }

        /// <summary>
        /// Abre la ventana para registrar un nuevo cliente. 
        /// Al cerrar la ventana, recarga los datos de la base de datos para mostrar el nuevo registro.
        /// </summary>
        private void btnNuevoCliente_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Creamos la instancia de la ventana
                WindowCliente ventanaCliente = new WindowCliente();

                // Abrir la ventana en modo diálogo (bloquea la anterior hasta que se cierre)
                ventanaCliente.ShowDialog();

                // Al cerrar la ventana, refrescamos la base de datos por si hubo cambios
                CargarDatos();
            } 
            catch (Exception ex) 
            {
                // Capturamos cualquier error inesperado
                MessageBox.Show($"Error al añadir cliente: {ex.Message}", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obtiene el cliente de la fila seleccionada en el DataGrid y abre la ventana de edición para editar ese cliente.
        /// </summary>
        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // El 'sender' es el botón que acabamos de pulsar.
                // Usamos 'as Button' para acceder a sus propiedades.
                Button botonPulsado = sender as Button;

                // Cada fila de la tabla tiene un 'DataContext', que es el objeto Cliente de esa fila.
                // Al extraer el DataContext del botón, obtenemos el cliente exacto de esa posición.
                Cliente clienteDeLaFila = botonPulsado.DataContext as Cliente;

                // Verificamos que hemos podido obtener el cliente correctamente
                if (clienteDeLaFila != null)
                {
                    // Creamos la instancia de la ventana pasándole el cliente
                    WindowCliente ventanaEdicion = new WindowCliente(clienteDeLaFila);

                    // Abrir la ventana en modo diálogo (bloquea la anterior hasta que se cierre)
                    ventanaEdicion.ShowDialog();

                    // Al cerrar la ventana, refrescamos la base de datos por si hubo cambios
                    CargarDatos();
                }
            } 
            catch (Exception ex) 
            {
                // Capturamos cualquier error inesperado
                MessageBox.Show($"Error al editar cliente: {ex.Message}", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Método que se ejecuta al pulsar el botón de eliminar en una fila del DataGrid.
        /// Obtiene el cliente asociado a esa fila, solicita confirmación al usuario y,
        /// si se confirma, elimina el cliente de la tabla clientes base de la datos vetcar.
        /// </summary>
        /// <param name="sender">Botón de eliminar que ha sido pulsado.</param>
        /// <param name="e">Argumentos del evento RoutedEvent.</param>
        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // El 'sender' es el botón que hemos pulsado dentro de una fila del DataGrid
                Button botonPulsado = sender as Button;

                // Obtenemos el cliente asociado a esa fila mediante el DataContext
                Cliente clienteDeLaFila = botonPulsado.DataContext as Cliente;

                // Verificamos que el cliente no sea nulo
                if (clienteDeLaFila != null)
                {
                    // Mostramos un mensaje de confirmación antes de eliminar
                    MessageBoxResult confirmacion = MessageBox.Show(
                        $"¿Estás seguro de que deseas eliminar al cliente {clienteDeLaFila.Nombre} {clienteDeLaFila.Apellidos}?",
                        "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    // Si el usuario pulsa "Yes", procedemos a eliminar
                    if (confirmacion == MessageBoxResult.Yes)
                    {
                        // Llamamos al servicio de negocio para eliminar el cliente por su id
                        bool eliminado = cs.Eliminar(clienteDeLaFila.IdCliente);

                        // Si se eliminó correctamente
                        if (eliminado)
                        {
                            MessageBox.Show("Cliente eliminado correctamente.", "Información",
                                            MessageBoxButton.OK, MessageBoxImage.Information);

                            // Recargamos los datos para actualizar la tabla
                            CargarDatos();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo eliminar el cliente.", "Error",
                                            MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Capturamos cualquier error inesperado
                MessageBox.Show($"Error al eliminar cliente: {ex.Message}", "Error", 
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Método que se ejecuta al pulsar el botón de eliminar varios clientes.
        /// Obtiene todos los clientes seleccionados en el DataGrid, solicita confirmación al usuario y, si se confirma, 
        /// elimina todos los registros seleccionados de la tabla clientes de la base de datos vetcare.
        /// </summary>
        /// <param name="sender">Botón que ha sido pulsado para eliminar múltiples clientes.</param>
        /// <param name="e">Argumentos del evento RoutedEvent.</param>
        private void btnEliminarVarios_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Verificamos que haya elementos seleccionados en el DataGrid
                if (dgClientes.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Debes seleccionar al menos un cliente para eliminar.", "Aviso",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Mostramos mensaje de confirmación indicando cuántos clientes se eliminarán
                MessageBoxResult confirmacion = MessageBox.Show(
                    $"¿Estás seguro de que deseas eliminar {dgClientes.SelectedItems.Count} cliente(s)?",
                    "Confirmar eliminación múltiple", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                // Si el usuario confirma
                if (confirmacion == MessageBoxResult.Yes)
                {
                    // Lista donde almacenaremos los IDs de los clientes seleccionados
                    List<int> idsAEliminar = new List<int>();

                    // Recorremos los elementos seleccionados
                    foreach (Cliente cliente in dgClientes.SelectedItems)
                    {
                        idsAEliminar.Add(cliente.IdCliente);
                    }

                    // Llamamos al servicio para eliminar todos los clientes seleccionados
                    bool eliminados = cs.EliminarVarias(idsAEliminar);

                    if (eliminados)
                    {
                        // Informamos de que se han eliminado los clientes
                        MessageBox.Show("Clientes eliminados correctamente.", "Información",
                                        MessageBoxButton.OK, MessageBoxImage.Information);

                        // Recargamos los datos para refrescar la tabla
                        CargarDatos();
                    }
                    else
                    {
                        // Informamos de que no se ha podido eliminar los clientes
                        MessageBox.Show("No se pudieron eliminar los clientes.", "Error",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                // Capturamos errores inesperados
                MessageBox.Show($"Error al eliminar clientes: {ex.Message}", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnVerFichaCliente_Click(object sender, RoutedEventArgs e)
        {
            // Corregido: Cast específico a Hyperlink
            var hyperlink = sender as Hyperlink;

            // Obtenemos el cliente desde el DataContext del Hyperlink
            var clienteSeleccionado = hyperlink?.DataContext as Cliente;

            if (clienteSeleccionado != null)
            {
                WindowFichaCliente win = new WindowFichaCliente(clienteSeleccionado.IdCliente);
                win.Owner = Window.GetWindow(this);
                win.ShowDialog();

                // Recargar grid si hubo cambios
                CargarDatos();
            }
        }

        private void dgClientes_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgClientes.SelectedItem is Cliente clienteSeleccionado)
            {
                WindowFichaCliente win = new WindowFichaCliente(clienteSeleccionado.IdCliente);
                win.Owner = Window.GetWindow(this);
                win.ShowDialog();

                CargarDatos();
            }
        }

        private void dgClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnEliminarVarios.Visibility =
                dgClientes.SelectedItems.Count > 0
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }
}