using System.Net.Http;
using System.Diagnostics;
using System.Data;
using System.Numerics;

namespace restoran_app
{
    public partial class MainForm : Form
    {
        List<Order> orders;
        DataTable ordersTable;
        // API's maybe null
        Trendyol? trendyolClient;
        Yemeksepeti? yemeksepetiClient;
        Getir? getirClient;
        public MainForm()
        {
            InitializeComponent();
            // Create orders
            orders = new List<Order>();
            // Create table
            ordersTable = new DataTable();
            // Create table columns, must match with Order structure
            ordersTable.Columns.Add("Müþteri Adý", typeof(string));
            ordersTable.Columns.Add("Tutar", typeof(string));
            ordersTable.Columns.Add("Sipariþ Tarihi", typeof(string));
            ordersTable.Columns.Add("Onaylanma Tarihi", typeof(string));
            ordersTable.Columns.Add("Sipariþ Durumu", typeof(string));
            dataGridView1.DataSource = ordersTable;
            // Disable sorting for columns
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            // initialize apis
            InitTrendyol();
            InitYemeksepeti();
            InitGetir();
            // First update, then timer updates
            FetchOrders();
        }
        private void InitTrendyol()
        {
            // Check credentials, only create them if the full credentials per api is available
            if (Credentials.Default.trendyolEmail != "" &&
                Credentials.Default.trendyolSellerID != "" &&
                Credentials.Default.trendyolApiKey != "" &&
                Credentials.Default.trendyolApiSecret != "")
            {
                trendyolClient = new Trendyol(
                    // "aliskingokhan@gmail.com", 561856, "d3p3mf2mirsu48cmIkeo", "oXugaiOFFWLElfTCVNO9"
                    Credentials.Default.trendyolEmail,
                    Credentials.Default.trendyolSellerID,
                    Credentials.Default.trendyolApiKey,
                    Credentials.Default.trendyolApiSecret
                );
            }
        }
        private void InitYemeksepeti()
        {
            if (Credentials.Default.yemeksepetiEmail != "" &&
                Credentials.Default.yemeksepetiPassword != "")
            {
                yemeksepetiClient = new Yemeksepeti(
                    Credentials.Default.yemeksepetiEmail,
                    Credentials.Default.yemeksepetiPassword
                );
            }
        }
        private void InitGetir()
        {
            if (Credentials.Default.getirRestaurantID != "" &&
                Credentials.Default.getirToken != "")
            {
                getirClient = new Getir(
                    Credentials.Default.getirRestaurantID,
                    Credentials.Default.getirToken
                );
            }
        }
        private void UpdateOrders(List<Order> orders)
        {
            // in order to keep selected order we save it's id
            BigInteger selectedID = new();
            int rowIndex = GetSelectedRowIndex();
            if (rowIndex >= 0)
            {
                selectedID = this.orders[rowIndex].id;
            }
            // set orders
            this.orders = orders;
            // sort orders
            this.orders.Sort((left, right) =>
            {
                if (left.packageStatus < right.packageStatus)
                {
                    return -1;
                }
                else if (left.packageStatus > right.packageStatus)
                {
                    return 1;
                }
                if (left.creationTime < right.creationTime)
                {
                    return 1;
                }
                else if (left.creationTime > right.creationTime)
                {
                    return -1;
                }
                return 0;
            });
            // check order status and accept new ones
            foreach (var order in orders)
            {
                // TODO
            }
            // update table
            UpdateOrdersTable();
            // set selected
            if (rowIndex >= 0)
            {
                // find new index
                int index = FindOrderIndexWithID(selectedID);
                if (index >= 0)
                {
                    dataGridView1.Rows[index].Selected = true;
                }
            }
        }
        private int FindOrderIndexWithID(BigInteger id)
        {
            for (int i = 0; i < this.orders.Count; i++)
            {
                if (this.orders[i].id == id)
                {
                    return i;
                }
            }
            return -1;
        }
        private void UpdateOrdersTable()
        {
            ordersTable.Rows.Clear();
            foreach (var order in orders)
            {
                ordersTable.Rows.Add(
                    order.customerDetails.name,
                    order.totalPrice.ToString(),
                    order.creationTime.ToString("dd.MM.yyyy HH:mm"),
                    order.pickedTime.ToString(), // TODO
                    order.PackageStatusString()
                );
            }
            // Table colors
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                var row = dataGridView1.Rows[i];
                var cell = row.Cells[row.Cells.Count - 1]; // last cell is status
                PackageStatus status = orders[i].packageStatus;
                switch (status)
                {
                    case PackageStatus.Created:
                        cell.Style.ForeColor = Color.LightGreen;
                        break;
                    case PackageStatus.Picking:
                        cell.Style.ForeColor = Color.Green;
                        break;
                    case PackageStatus.Invoiced:
                        cell.Style.ForeColor = Color.BlueViolet;
                        break;
                    case PackageStatus.Cancelled:
                        cell.Style.ForeColor = Color.DarkRed;
                        break;
                    case PackageStatus.Unsupplied: // IDK
                        break;
                    case PackageStatus.Shipped:
                        cell.Style.ForeColor = Color.LightBlue;
                        break;
                    case PackageStatus.Delivered:
                        cell.Style.ForeColor = Color.Blue;
                        break;
                    default:
                        break;
                }
            }
        }
        private async void FetchOrders()
        {
            SetFormStatusText("Güncelleniyor...");
            // Check every client
            if (trendyolClient == null && yemeksepetiClient == null && getirClient == null)
            {
                SetFormStatusText("Hiçbir API ayarý yapýlmadý. Veriler çekilemiyor.");
                return;
            }
            bool errors = false;
            // We have some of the api's, fetch non-null ones
            Task<List<Order>>? trendyolOrdersTask = null;
            if (trendyolClient != null)
            {
                trendyolOrdersTask = trendyolClient.FetchOrders();
            }
            Task<List<Order>>? yemeksepetiOrdersTask = null;
            if (yemeksepetiClient != null)
            {
                yemeksepetiOrdersTask = yemeksepetiClient.FetchOrders();
            }
            Task<List<Order>>? getirOrdersTask = null;
            if (getirClient != null)
            {
                getirOrdersTask = getirClient.FetchOrders();
            }
            // We fetched from all non-null apis. Await and merge their orders
            List<Order> orders = new List<Order>();
            if (trendyolOrdersTask != null)
            {
                try
                {
                    orders.AddRange(await trendyolOrdersTask);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                    errors = true;
                }
            }
            if (yemeksepetiOrdersTask != null)
            {
                try
                {
                    orders.AddRange(await yemeksepetiOrdersTask);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                    errors = true;
                }
            }
            if (getirOrdersTask != null)
            {
                try
                {
                    orders.AddRange(await getirOrdersTask);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                    errors = true;
                }
            }
            // update orders
            UpdateOrders(orders);
            // set status if errors, just for debugging
            if (errors)
            {
                SetFormStatusText("Hatalar var, herþey güncel olmayabilir");
            }
            else
            {
                SetFormStatusText("Güncel");
            }
            // reset timer
            timer1.Stop();
            timer1.Start();
        }
        private int GetSelectedRowIndex()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                return dataGridView1.SelectedRows[0].Index;
            }
            return -1;
        }
        private void SetFormStatusText(string text)
        {
            toolStripStatusLabel1.Text = text;
        }
        private void SetSelectedOrder(Order order)
        {
            // Clear all lists
            menuDetailsTree.Nodes.Clear();
            customerDetailsList.Items.Clear();
            orderDetailsList.Items.Clear();
            // Set menu details
            SetOrderMenuDetails(order.items, null);
            menuDetailsTree.ExpandAll();
            // Set customer details
            customerDetailsList.Items.Add(new ListViewItem(new[] { "Ýsim", order.customerDetails.name }));
            customerDetailsList.Items.Add(new ListViewItem(new[] { "Adres", order.customerDetails.address }));
            customerDetailsList.Items.Add(new ListViewItem(new[] { "Apartman No", order.customerDetails.apartmentNumber }));
            customerDetailsList.Items.Add(new ListViewItem(new[] { "Kapý No", order.customerDetails.doorNumber }));
            customerDetailsList.Items.Add(new ListViewItem(new[] { "Mahalle", order.customerDetails.neighborhood }));
            customerDetailsList.Items.Add(new ListViewItem(new[] { "Ýlçe", order.customerDetails.district }));
            customerDetailsList.Items.Add(new ListViewItem(new[] { "Þehir", order.customerDetails.city }));
            customerDetailsList.Items.Add(new ListViewItem(new[] { "Posta Kodu", order.customerDetails.postalCode }));
            customerDetailsList.Items.Add(new ListViewItem(new[] { "Telefon", order.customerDetails.phone }));
            customerDetailsList.Items.Add(new ListViewItem(new[] { "Not", order.customerDetails.note }));
            customerDetailsList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            // Set package details
            orderDetailsList.Items.Add(new ListViewItem(new[] { "API", order.api.ToString() }));
            orderDetailsList.Items.Add(new ListViewItem(new[] { "Original ID", order.origID.ToString() }));
            orderDetailsList.Items.Add(new ListViewItem(new[] { "ID", order.id.ToString() }));
            orderDetailsList.Items.Add(new ListViewItem(new[] { "Status", order.packageStatus.ToString() }));
            orderDetailsList.Items.Add(new ListViewItem(new[] { "Durum", order.PackageStatusString() }));
            orderDetailsList.Items.Add(new ListViewItem(new[] { "Müþteri Notu", order.customerNote }));
            orderDetailsList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            // Set button name and action according to status
            switch (order.packageStatus)
            {
                case PackageStatus.Picking:
                    actionButton.Text = "Hazýrlandý";
                    actionButton.Enabled = true;
                    break;
                case PackageStatus.Invoiced:
                    actionButton.Text = "Yola çýktý";
                    actionButton.Enabled = true;
                    break;
                case PackageStatus.Shipped:
                    actionButton.Text = "Teslim edildi";
                    actionButton.Enabled = true;
                    break;
                default:
                    actionButton.Text = "Ýþlem yok";
                    actionButton.Enabled = false;
                    break;
            }
        }
        public void SetOrderMenuDetails(List<Item> items, TreeNode? parent)
        {
            foreach (var item in items)
            {
                TreeNode node = new TreeNode(item.description.name + " - " + item.description.price.ToString());
                if (item.modifiers.Count > 0)
                {
                    SetOrderMenuDetails(item.modifiers, node);
                }
                if (item.extras.Count > 0)
                {
                    TreeNode extras = new TreeNode("Extralar");
                    foreach (var extra in item.extras)
                    {
                        extras.Nodes.Add(new TreeNode(extra.name + " - " + extra.price));
                    }
                    node.Nodes.Add(extras);
                }
                if (item.removed.Count > 0)
                {
                    TreeNode removeds = new TreeNode("Çýkarýlanlar");
                    foreach (var removed in item.removed)
                    {
                        removeds.Nodes.Add(new TreeNode(removed.name));
                    }
                    node.Nodes.Add(removeds);
                }
                if (parent == null)
                {
                    menuDetailsTree.Nodes.Add(node);
                }
                else
                {
                    parent.Nodes.Add(node);
                }
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            FetchOrders();
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            int index = GetSelectedRowIndex();
            if (index >= 0)
            {
                SetSelectedOrder(this.orders[index]);
            }
        }
        private void ayarlarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }
        private void yenileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FetchOrders();
        }
        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.ShowLogForm();
        }
    }
}