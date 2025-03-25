namespace MarketManagement.UserControls
{
    partial class Order
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txt_customerId = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_search_by_customer_Id = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btn_search_by_invoiceno = new System.Windows.Forms.Button();
            this.txt_invoiceno = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.db_ordersDataGridView = new System.Windows.Forms.DataGridView();
            this.invoiceDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.invoiceNoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.customerIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.customerNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contactDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.addressDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grandTotalDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.orderBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel3 = new System.Windows.Forms.Panel();
            this.btn_refresh = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.db_ordersDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.orderBindingSource)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_customerId
            // 
            this.txt_customerId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_customerId.Location = new System.Drawing.Point(24, 49);
            this.txt_customerId.Name = "txt_customerId";
            this.txt_customerId.Size = new System.Drawing.Size(325, 26);
            this.txt_customerId.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(120)))), ((int)(((byte)(153)))));
            this.label6.Location = new System.Drawing.Point(21, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(97, 18);
            this.label6.TabIndex = 11;
            this.label6.Text = "Customer ID";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btn_search_by_customer_Id);
            this.panel1.Controls.Add(this.txt_customerId);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Location = new System.Drawing.Point(567, 91);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(482, 107);
            this.panel1.TabIndex = 13;
            // 
            // btn_search_by_customer_Id
            // 
            this.btn_search_by_customer_Id.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(221)))), ((int)(((byte)(143)))));
            this.btn_search_by_customer_Id.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btn_search_by_customer_Id.FlatAppearance.BorderSize = 2;
            this.btn_search_by_customer_Id.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(50)))), ((int)(((byte)(93)))));
            this.btn_search_by_customer_Id.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btn_search_by_customer_Id.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_search_by_customer_Id.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_search_by_customer_Id.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btn_search_by_customer_Id.Location = new System.Drawing.Point(364, 42);
            this.btn_search_by_customer_Id.Name = "btn_search_by_customer_Id";
            this.btn_search_by_customer_Id.Size = new System.Drawing.Size(98, 36);
            this.btn_search_by_customer_Id.TabIndex = 15;
            this.btn_search_by_customer_Id.Text = "Search";
            this.btn_search_by_customer_Id.UseVisualStyleBackColor = false;
            this.btn_search_by_customer_Id.Click += new System.EventHandler(this.btn_search_by_customer_Id_Click);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btn_search_by_invoiceno);
            this.panel2.Controls.Add(this.txt_invoiceno);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(53, 91);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(482, 107);
            this.panel2.TabIndex = 14;
            // 
            // btn_search_by_invoiceno
            // 
            this.btn_search_by_invoiceno.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(120)))), ((int)(((byte)(153)))));
            this.btn_search_by_invoiceno.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btn_search_by_invoiceno.FlatAppearance.BorderSize = 2;
            this.btn_search_by_invoiceno.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(50)))), ((int)(((byte)(93)))));
            this.btn_search_by_invoiceno.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btn_search_by_invoiceno.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_search_by_invoiceno.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_search_by_invoiceno.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btn_search_by_invoiceno.Location = new System.Drawing.Point(363, 43);
            this.btn_search_by_invoiceno.Name = "btn_search_by_invoiceno";
            this.btn_search_by_invoiceno.Size = new System.Drawing.Size(98, 36);
            this.btn_search_by_invoiceno.TabIndex = 14;
            this.btn_search_by_invoiceno.Text = "Search";
            this.btn_search_by_invoiceno.UseVisualStyleBackColor = false;
            this.btn_search_by_invoiceno.Click += new System.EventHandler(this.btn_search_by_invoiceno_Click);
            // 
            // txt_invoiceno
            // 
            this.txt_invoiceno.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_invoiceno.Location = new System.Drawing.Point(24, 49);
            this.txt_invoiceno.Name = "txt_invoiceno";
            this.txt_invoiceno.Size = new System.Drawing.Size(325, 26);
            this.txt_invoiceno.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(172)))), ((int)(((byte)(189)))));
            this.label2.Location = new System.Drawing.Point(21, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 18);
            this.label2.TabIndex = 11;
            this.label2.Text = "Invoice ID";
            // 
            // db_ordersDataGridView
            // 
            this.db_ordersDataGridView.AllowUserToAddRows = false;
            this.db_ordersDataGridView.AutoGenerateColumns = false;
            this.db_ordersDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.db_ordersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.db_ordersDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.invoiceDateDataGridViewTextBoxColumn,
            this.invoiceNoDataGridViewTextBoxColumn,
            this.customerIdDataGridViewTextBoxColumn,
            this.customerNameDataGridViewTextBoxColumn,
            this.contactDataGridViewTextBoxColumn,
            this.addressDataGridViewTextBoxColumn,
            this.grandTotalDataGridViewTextBoxColumn});
            this.db_ordersDataGridView.DataSource = this.orderBindingSource;
            this.db_ordersDataGridView.Location = new System.Drawing.Point(3, 3);
            this.db_ordersDataGridView.MultiSelect = false;
            this.db_ordersDataGridView.Name = "db_ordersDataGridView";
            this.db_ordersDataGridView.RowHeadersWidth = 62;
            this.db_ordersDataGridView.RowTemplate.Height = 28;
            this.db_ordersDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.db_ordersDataGridView.Size = new System.Drawing.Size(1111, 296);
            this.db_ordersDataGridView.TabIndex = 0;
            // 
            // invoiceDateDataGridViewTextBoxColumn
            // 
            this.invoiceDateDataGridViewTextBoxColumn.DataPropertyName = "InvoiceDate";
            this.invoiceDateDataGridViewTextBoxColumn.HeaderText = "InvoiceDate";
            this.invoiceDateDataGridViewTextBoxColumn.MinimumWidth = 6;
            this.invoiceDateDataGridViewTextBoxColumn.Name = "invoiceDateDataGridViewTextBoxColumn";
            // 
            // invoiceNoDataGridViewTextBoxColumn
            // 
            this.invoiceNoDataGridViewTextBoxColumn.DataPropertyName = "InvoiceNo";
            this.invoiceNoDataGridViewTextBoxColumn.HeaderText = "InvoiceNo";
            this.invoiceNoDataGridViewTextBoxColumn.MinimumWidth = 6;
            this.invoiceNoDataGridViewTextBoxColumn.Name = "invoiceNoDataGridViewTextBoxColumn";
            // 
            // customerIdDataGridViewTextBoxColumn
            // 
            this.customerIdDataGridViewTextBoxColumn.DataPropertyName = "CustomerId";
            this.customerIdDataGridViewTextBoxColumn.HeaderText = "CustomerId";
            this.customerIdDataGridViewTextBoxColumn.MinimumWidth = 6;
            this.customerIdDataGridViewTextBoxColumn.Name = "customerIdDataGridViewTextBoxColumn";
            // 
            // customerNameDataGridViewTextBoxColumn
            // 
            this.customerNameDataGridViewTextBoxColumn.DataPropertyName = "CustomerName";
            this.customerNameDataGridViewTextBoxColumn.HeaderText = "CustomerName";
            this.customerNameDataGridViewTextBoxColumn.MinimumWidth = 6;
            this.customerNameDataGridViewTextBoxColumn.Name = "customerNameDataGridViewTextBoxColumn";
            // 
            // contactDataGridViewTextBoxColumn
            // 
            this.contactDataGridViewTextBoxColumn.DataPropertyName = "Contact";
            this.contactDataGridViewTextBoxColumn.HeaderText = "Contact";
            this.contactDataGridViewTextBoxColumn.MinimumWidth = 6;
            this.contactDataGridViewTextBoxColumn.Name = "contactDataGridViewTextBoxColumn";
            // 
            // addressDataGridViewTextBoxColumn
            // 
            this.addressDataGridViewTextBoxColumn.DataPropertyName = "Address";
            this.addressDataGridViewTextBoxColumn.HeaderText = "Address";
            this.addressDataGridViewTextBoxColumn.MinimumWidth = 6;
            this.addressDataGridViewTextBoxColumn.Name = "addressDataGridViewTextBoxColumn";
            // 
            // grandTotalDataGridViewTextBoxColumn
            // 
            this.grandTotalDataGridViewTextBoxColumn.DataPropertyName = "GrandTotal";
            this.grandTotalDataGridViewTextBoxColumn.HeaderText = "GrandTotal";
            this.grandTotalDataGridViewTextBoxColumn.MinimumWidth = 6;
            this.grandTotalDataGridViewTextBoxColumn.Name = "grandTotalDataGridViewTextBoxColumn";
            // 
            // orderBindingSource
            // 
            this.orderBindingSource.DataSource = typeof(MarketManagement.Model.Order);
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel3.Controls.Add(this.db_ordersDataGridView);
            this.panel3.Location = new System.Drawing.Point(0, 321);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1117, 341);
            this.panel3.TabIndex = 32;
            // 
            // btn_refresh
            // 
            this.btn_refresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(222)))), ((int)(((byte)(249)))));
            this.btn_refresh.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btn_refresh.FlatAppearance.BorderSize = 2;
            this.btn_refresh.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(110)))), ((int)(((byte)(173)))));
            this.btn_refresh.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(173)))), ((int)(((byte)(217)))));
            this.btn_refresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_refresh.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_refresh.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(58)))), ((int)(((byte)(94)))));
            this.btn_refresh.Location = new System.Drawing.Point(481, 233);
            this.btn_refresh.Name = "btn_refresh";
            this.btn_refresh.Size = new System.Drawing.Size(137, 41);
            this.btn_refresh.TabIndex = 30;
            this.btn_refresh.Text = "Refresh";
            this.btn_refresh.UseVisualStyleBackColor = false;
            this.btn_refresh.Click += new System.EventHandler(this.btn_refresh_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 11.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(58)))), ((int)(((byte)(94)))));
            this.label1.Location = new System.Drawing.Point(390, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(337, 18);
            this.label1.TabIndex = 33;
            this.label1.Text = "Enter the invoice ID or customer ID to search";
            // 
            // Order
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(246)))));
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.btn_refresh);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Order";
            this.Size = new System.Drawing.Size(1117, 696);
            this.Load += new System.EventHandler(this.UC_orders_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.db_ordersDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.orderBindingSource)).EndInit();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion  
        private System.Windows.Forms.TextBox txt_customerId;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txt_invoiceno;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_search_by_customer_Id;
        private System.Windows.Forms.Button btn_search_by_invoiceno;
        private System.Windows.Forms.DataGridView db_ordersDataGridView;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btn_refresh;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn invoiceDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn invoiceNoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn customerIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn customerNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn contactDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn addressDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn grandTotalDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource orderBindingSource;
    }
}
